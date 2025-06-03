using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
// ReSharper disable ConvertConstructorToMemberInitializers

namespace Readora.Services;

public class BlockchainService
{
    
    private readonly Web3 _web3;
    private readonly string _contractAddress;
    private readonly string _abi;

    public BlockchainService()
    {
        _web3 = new Web3("http://localhost:7545"); 
        _contractAddress = "0xE24acB9a826102A20F1Cd1035a8E4Eb50F3d3493";
        _abi = """
               [{"inputs": [
               		{
               			"internalType": "uint256",
               			"name": "_id",
               			"type": "uint256"
               		},
               		{
               			"internalType": "string",
               			"name": "_title",
               			"type": "string"
               		},
               		{
               			"internalType": "string",
               			"name": "_fileHash",
               			"type": "string"
               		}
               	],
               		"name": "addBook",
               		"outputs": [],
               		"stateMutability": "nonpayable",
               		"type": "function"
               	},
               	{
               		"anonymous": false,
               		"inputs": [
               			{
               				"indexed": false,
               				"internalType": "uint256",
               				"name": "id",
               				"type": "uint256"
               			},
               			{
               				"indexed": false,
               				"internalType": "string",
               				"name": "title",
               				"type": "string"
               			},
               			{
               				"indexed": false,
               				"internalType": "string",
               				"name": "fileHash",
               				"type": "string"
               			}
               		],
               		"name": "BookAdded",
               		"type": "event"
               	},
               	{
               		"inputs": [],
               		"name": "bookCount",
               		"outputs": [
               			{
               				"internalType": "uint256",
               				"name": "",
               				"type": "uint256"
               			}
               		],
               		"stateMutability": "view",
               		"type": "function"
               	},
               	{
               		"inputs": [
               			{
               				"internalType": "uint256",
               				"name": "",
               				"type": "uint256"
               			}
               		],
               		"name": "books",
               		"outputs": [
               			{
               				"internalType": "uint256",
               				"name": "id",
               				"type": "uint256"
               			},
               			{
               				"internalType": "string",
               				"name": "title",
               				"type": "string"
               			},
               			{
               				"internalType": "string",
               				"name": "fileHash",
               				"type": "string"
               			}
               		],
               		"stateMutability": "view",
               		"type": "function"
               	},
               	{
               		"inputs": [
               			{
               				"internalType": "uint256",
               				"name": "_id",
               				"type": "uint256"
               			}
               		],
               		"name": "getBook",
               		"outputs": [
               			{
               				"internalType": "uint256",
               				"name": "",
               				"type": "uint256"
               			},
               			{
               				"internalType": "string",
               				"name": "",
               				"type": "string"
               			},
               			{
               				"internalType": "string",
               				"name": "",
               				"type": "string"
               			}
               		],
               		"stateMutability": "view",
               		"type": "function"
               	}
               ]
               """;
    }

    public async Task<string?> AddBookAsync(int bookId, string title, string fileHash)
    {
        var contract = _web3.Eth.GetContract(_abi, _contractAddress);
        try
        {
	        // 4. Получаем аккаунты
	        var accounts = await _web3.Eth.Accounts.SendRequestAsync();
	        var account = accounts[0]; // Первый аккаунт Ganache

	        // 5. Формируем транзакцию
	        var addBookFunction = contract.GetFunction("addBook");
	        var gasEstimate = await addBookFunction.EstimateGasAsync(
		        from: account,
		        gas: null,
		        value: null,
		        functionInput: [bookId, title, fileHash]
	        );

	        var txHash = await addBookFunction.SendTransactionAsync(
		        from: account,
		        gas: gasEstimate,
		        value: null,
		        functionInput: [bookId, title, fileHash]
	        );
	        
	        return txHash;
        }
        catch (Exception ex)
        {
	        Console.WriteLine($"Error: {ex.Message}");
	        return null;
        }
    }
    
    public async Task<BigInteger> GetBookCount()
    {
	    var contract = _web3.Eth.GetContract(_abi, _contractAddress);
	    var function = contract.GetFunction("bookCount");
	    return await function.CallAsync<BigInteger>();
    }

    public async Task<List<object>> GetBook(BigInteger id)
    {
	    var contract = _web3.Eth.GetContract(_abi, _contractAddress);
	    var function = contract.GetFunction("getBook");
    
	    var result = await function.CallDeserializingToObjectAsync<BookResult>(id);
	    return [result.Id, result.Title, result.FileHash];
    }

    private class BookResult
    {
	    [Parameter("uint256", "id", 1)]
	    public BigInteger Id { get; set; }

	    [Parameter("string", "title", 2)]
	    public string Title { get; set; }

	    [Parameter("string", "fileHash", 3)]
	    public string FileHash { get; set; }
    }
}