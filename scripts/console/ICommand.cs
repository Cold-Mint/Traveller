using System.Threading.Tasks;

namespace ColdMint.scripts.console;

public interface ICommand
{
    string Name { get; }

    Task<bool> Execute(string[] args);
}