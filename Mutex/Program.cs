using System;
using System.Collections.Generic;
using System.Threading;

namespace MutexExample {
    internal class Program {
        static void Main(string[] args) {
            Mutex mutex = new Mutex();
            Bank bank = new Bank(100);

            List<Person> people = new List<Person>() {
                new Person(bank, "Juan", mutex),
                new Person(bank, "Pedro", mutex),
                new Person(bank, "Luis", mutex),
                new Person(bank, "Jesus", mutex),
                new Person(bank, "Maria", mutex),
                new Person(bank, "Ana", mutex),
            };
            people.ForEach(p => new Thread(new ThreadStart(p.Withdraw)).Start());
            Console.ReadKey();
        }
    }
    public class Person {
        private string name;
        private Bank bank;
        private Mutex mutex;
        public Person(Bank bank, string name, Mutex mutex) {
            this.name = name;
            this.bank = bank;
            this.mutex = mutex;
        }
        public void Withdraw() {
            while (bank.Funds > 0) {
                try {
                    mutex.WaitOne();
                    if (bank.Funds > 0) {
                        int value = new Random().Next(1, 5);
                        Console.WriteLine($"Funds from bank: ${bank.Funds}.");
                        bank.Withdrawals(value);
                        Console.WriteLine($"{name} withdrew ${value} and now there is ${bank.Funds} in the bank ");
                        mutex.ReleaseMutex();
                    }
                } catch (AbandonedMutexException ex) {
                    Console.WriteLine("There are not enough funds in the bank");
                }
            }
        }
    }
    public class Bank {
        public int Funds { get; private set; }
        public Bank(int funds) => this.Funds = funds;
        public void Withdrawals(int value) => Funds -= value;
    }
}