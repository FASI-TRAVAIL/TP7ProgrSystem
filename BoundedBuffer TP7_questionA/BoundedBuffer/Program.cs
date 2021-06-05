
using System;
using System.Threading;

public class BoundedBuffer
{
   
    private const int BUFFER_SIZE = 5;
    private Object[] buffer;
    private int IN, OUT;
    private Semaphore mutex, empty, full;

    public BoundedBuffer()
    {
        IN = 0;
        OUT = 0;
        buffer = new Object[BUFFER_SIZE];

    }

    public void insert(Object item)
    {
        empty.WaitOne();
        mutex.WaitOne();
        // put an item in the buffer
        buffer[IN] = item;
        IN = (IN + 1) % BUFFER_SIZE;
        mutex.Release();
        full.Release();
    }


    public Object remove()
    {
        full.WaitOne();
        mutex.WaitOne();
        // remove an item from the buffer  
        Object item = buffer[OUT];
        OUT = (OUT + 1) % BUFFER_SIZE;
        mutex.Release();
        empty.Release();
        return item;
    }

    public void WriteData(double valeur)
    {
        Console.WriteLine("Value is : "+valeur);
       
    }

    internal void ReadData(out double pi)
    {
        pi = 0;
    }

}

class MyProducer
{
    private Random rand = new Random(1);
    private BoundedBuffer boundedBuffer;
    private int totalIters;

    public MyProducer(BoundedBuffer boundedBuffer, int iterations)
    {
        this.boundedBuffer = boundedBuffer;
        totalIters = iterations;
    }

    public Thread CreateProducerThread()
    {
        return new Thread(new ThreadStart(this.calculate));
    }
    private void calculate()
    {
        int iters = 0;
        do
        {
            iters += 1;
            Thread.Sleep(rand.Next(2000));
            boundedBuffer.WriteData(iters * 4);
        } while (iters < totalIters);
    }
}

class MyConsumer
{
    private Random rand = new Random(2);
    private BoundedBuffer boundedBuffer;
    private int totalIters;


    public MyConsumer(BoundedBuffer boundedBuffer, int iterations)
    {
        this.boundedBuffer = boundedBuffer;
        totalIters = iterations;
    }

    public Thread CreateConsumerThread()
    {
        return new Thread(new ThreadStart(this.printValues));
    }

    public void printValues()
    {
        int iters = 0;
        double pi;
        do
        {
            Thread.Sleep(rand.Next(2000));
            boundedBuffer.ReadData(out pi);
           // System.Console.WriteLine("Value {0} is: {1}", iters, pi.ToString());
            iters++;
        } while (iters < totalIters);
      //  System.Console.WriteLine("Done");
    }
}

class MainClass
{
    static void Main(string[] args)
    {
        BoundedBuffer boundedBuffer = new BoundedBuffer();

        MyProducer prod = new MyProducer(boundedBuffer, 20);
        Thread producerThread = prod.CreateProducerThread();

        MyConsumer cons = new MyConsumer(boundedBuffer, 20);
        Thread consumerThread = cons.CreateConsumerThread();

        producerThread.Start();
        consumerThread.Start();

        Console.ReadLine();
    }
}


