using System.Collections.Generic;
using System.Linq;

namespace XtraSubReports.TestResources.Models
{
    public class Dog
    {
        public Dog(string name, params string[] toys)
        {
            Name = name;
            CreateDogToys(toys);
        }

        public string Name { get; set; }
        public List<DogToy> DogToys { get; set; }

        private void CreateDogToys(params string[] toys)
        {
            DogToys = toys.ToArray().Select(o => new DogToy() { Name = o }).ToList();
        }
    }

    public class DogToy
    {
        public string Name { get; set; }
    }
}
