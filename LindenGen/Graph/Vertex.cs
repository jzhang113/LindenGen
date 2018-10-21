namespace LindenGen.Graph
{
    public class Vertex<T>
    {
        public int ID { get; set; }
        public T Data { get; set; }
        public bool Seen { get; set; }

        public Vertex(int id, T data)
        {
            ID = id;
            Data = data;
            Seen = false;
        }
    }
}
