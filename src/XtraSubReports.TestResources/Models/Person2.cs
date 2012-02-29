using System.Collections.Generic;

namespace XtraSubReports.TestResources.Models
{
    public class Person2
    {

        public static List<Person2> SampleData()
        {
            return new List<Person2>
                                    {
                                        new Person2
                                            {
                                                Name = "Douglas Sam",
                                                Age = 17,
                                                Dogs = new List<Dog>
                                                {
                                                    new Dog("Rex", "ball1", "ball2"), new Dog("Rudy", "ball3", "ball4")
                                                }
                                            },
                                        new Person2
                                            {
                                                Name = "Fred Thomas",
                                                Age = 35,
                                                Dogs =
                                                    new List<Dog> {new Dog("Sally", "ball5", "ball6"), new Dog("Stubert", "ball7", "ball8")}
                                            },
                                        new Person2
                                            {
                                                Name = "Alex Matthew",
                                                Age = 100,
                                                Dogs =
                                                    new List<Dog>
                                                        {new Dog("Nibbles", "ball9", "ball10"), new Dog("Norbert", "ball11", "ball12")}
                                            }

                                    };
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public List<Dog> Dogs { get; set; }
    }
}