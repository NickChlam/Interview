namespace FullStackDevExercise.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}