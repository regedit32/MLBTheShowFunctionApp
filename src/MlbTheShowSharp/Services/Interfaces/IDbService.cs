using MLBTheShowSharp.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal interface IDbService: IDisposable
    {
        Task AddItemAsync<T>(T item) where T : IItem, new();
    }
}