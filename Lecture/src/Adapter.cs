// The Target interface represents the domain-specific interface used by the client code.
public interface ITarget
{
    string GetRequest();
}

// The Adaptee contains some useful behavior, but its interface is incompatible
// with the existing client code.
public class Adaptee
{
    public string GetSpecificRequest()
    {
        return "Specific request";
    }
}

// The Adapter makes the Adaptee's interface compatible with the Target's interface.
public class Adapter : ITarget
{
    private readonly Adaptee adaptee;

    public Adapter(Adaptee adaptee)
    {
        this.adaptee = adaptee;
    }

    public string GetRequest()
    {
        return $"This is '{this.adaptee.GetSpecificRequest()}'";
    }
}

// The client code works with all objects that implement the Target interface.
public class Client
{
    public void Main()
    {
        Adaptee adaptee = new Adaptee();
        ITarget target = new Adapter(adaptee);

        Console.WriteLine(target.GetRequest());
    }
}

// class Program // Usage
// {
//     static void Main(string[] args)
//     {
//         new Client().Main();
//     }
// }

// - `ITarget` is the interface expected by the client.​
// - `Adaptee` is the class with an incompatible interface.​
// - `Adapter` makes `Adaptee`'s interface compatible with `ITarget`.​
// - `Client` uses the `ITarget` interface to interact with the `Adaptee` through the `Adapter`.