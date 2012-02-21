using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeniusCode.Framework.Extensions;
using GeniusCode.Framework.Support.Refection;

namespace GeniusCode.XtraReports.Design.Traversals
{
    /// <summary>
    /// Example path:  this.is[9].a.test
    /// </summary>
    public class ObjectGraphPathTraverser : IDataSourceTraverser
    {

        #region Instance

        const string PathDelimiter = ".";

        public object Traverse(object rootDataSource, string fullPath)
        {
            var target = rootDataSource;

            var members = BreakPathIntoMembers(fullPath).ToList();

            int index = 0;
            members.ForEach(member =>
            {
                bool isLastPathSegment = index == members.Count - 1;

                if (target != null)
                    target = TraverseMember(target, member, isLastPathSegment);

                index++;
            });

            return target;
        }

        private IEnumerable<MemberTraversal> BreakPathIntoMembers(string path)
        {
            if (path == null)
                return new MemberTraversal[] { };

            var split = path.Split(PathDelimiter);

            return split.Select(segment => ConvertPathSegmentToTraversal(segment));
        }

        private MemberTraversal ConvertPathSegmentToTraversal(string segment)
        {
            Match match = null;

            try
            {
                // parses "collection[0]" into member name & index
                var regex = new Regex("^(?<memberName>\\w*)(?:\\[(?<collectionIndex>[0-9]+)\\])?$");
                match = regex.Match(segment);
            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
                // Stop parsing path members
                return null;
            }

            var memberName = match.Groups["memberName"].Value;
            var collectionIndex = match.Groups["collectionIndex"].Value;

            if (collectionIndex != string.Empty)
                return new ExtractFromCollectionTraversal() { MemberName = memberName, IndexToExtract = int.Parse(collectionIndex) };
            else
                return new MemberTraversal() { MemberName = memberName };
        }

        private object TraverseMember(object target, MemberTraversal member, bool isLastPathSegment)
        {
            if (target == null) return null;

            object result = null;

            // Is Target a Collection?
            // Extract first item
            target.TryAs<IEnumerable>(collection =>
            {
                var list = Enumerable.Cast<object>(collection);
                var newTarget = list.FirstOrDefault();
                result = TraverseMember(newTarget, member, isLastPathSegment);
            });


            if (result == null)
            {
                // Target is a single object

                if (string.IsNullOrEmpty(member.MemberName))
                    // Self
                    result = target;
                else
                    // Get Related Member
                    result = ReflectionHelper.GetMemberValue(target, member.MemberName);

                // Is it a Collection?
                result.TryAs<IEnumerable>(collection =>
                {
                    // If not last path segment
                    // or supposed to extract item
                    bool shouldExtractItem = !isLastPathSegment || member is ExtractFromCollectionTraversal;

                    if (shouldExtractItem)
                        result = ExtractItemFromCollection(collection.Cast<object>().ToList(), member);
                });
            }

            return result;
        }



        private object ExtractItemFromCollection(List<object> collection, MemberTraversal member)
        {
            object result = null;

            if (collection == null || collection.Count == 0)
                return null;

            // By default, Extract first item
            result = collection.First();

            // or, Extract specific item from collection
            member.TryAs<ExtractFromCollectionTraversal>(extractTraversal =>
            {
                result = collection[extractTraversal.IndexToExtract];
            });

            return result;
        }

        #endregion

        TraversedDatasourceResult IDataSourceTraverser.TraversePath(object datasource, string path)
        {
            var targetDataSource = Traverse(datasource, path);
            return new TraversedDatasourceResult(datasource, targetDataSource);
        }
    }
}