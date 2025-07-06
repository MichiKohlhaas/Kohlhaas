using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Kohlhaas.Engine;

internal static class DataAccessUtility
{
    
    internal static void CreateFileIfMissing(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
    }

    internal static void CreateDirectoryIfMissing(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
    
    internal static async Task<Result> WriteToFileAsync<T>(string filePath, T content, ILogger logger, JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions { WriteIndented = true };
       
        var json = JsonSerializer.Serialize(content, options);
        await using var sw = File.CreateText(filePath);
        var result = await ExecuteStream<StreamWriter, Task>(sw, async (writer) => await writer.WriteAsync(json));
        if (result.IsSuccess == false) LogError(result.Error, logger);
        return result;
    }

    internal static async Task<T> ReadFromFileAsync<T>(string filePath, ILogger logger, JsonSerializerOptions? options = null)
    {
        T obj;
        options ??= new JsonSerializerOptions { WriteIndented = true };
        using var sr = new StreamReader(filePath);
        var result = await ExecuteStream<StreamReader, string>(sr, async (reader) => await reader.ReadToEndAsync());

        switch (result.Item2.IsSuccess)
        {
            case true:
                obj = JsonSerializer.Deserialize<T>(result.Item1, options) ?? default!;
                break;
            case false:
                obj = default!;
                LogError(result.Item2.Error, logger);
                break;
        }
        
        return obj;
    }

    private static void LogError(Error error, ILogger logger)
    {
        logger.LogError(error.Message);
    }

    private static async Task<(T, Result)> ExecuteStream<TStream, T>(TStream stream, Func<TStream, Task<T>> operation)
        where TStream : IDisposable
    {
        T result = default!;
        try
        {
            result = await operation(stream);
            return (result, Result.Success());
        }
        catch (FileNotFoundException e)
        {
            return (result, Result.Failure(new Error(e.HResult.ToString(),"The file or directory cannot be found.")));
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("The file or directory cannot be found.");
            return (result, Result.Failure(new Error(e.HResult.ToString(),"The directory cannot be found.")));
        }
        catch (DriveNotFoundException e)
        {
            Console.WriteLine("The drive specified in 'path' is invalid.");
            return (result, Result.Failure(new Error(e.HResult.ToString(),"The drive specified in 'path' is invalid.")));
        }
        catch (PathTooLongException e)
        {
            Console.WriteLine("'path' exceeds the maximum supported path length.");
            return (result, Result.Failure(new Error(e.HResult.ToString(),"The path is too long.")));
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("You do not have permission to create this file.");
            return (result, Result.Failure(new Error(e.HResult.ToString(),"You do not have permission to create this file.")));
        }
        catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
        {
            Console.WriteLine("There is a sharing violation.");
            return (result, Result.Failure(new Error(e.HResult.ToString(),"There is a sharing violation.")));
        }
        catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
        {
            Console.WriteLine("The file already exists.");
            return (result, Result.Failure(new Error(e.HResult.ToString(),"The file already exists.")));
        }
        catch (IOException e)
        {
            return (result, Result.Failure(new Error(e.HResult.ToString(),e.Message)));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (result, Result.Failure(new Error(e.HResult.ToString(),e.Message)));
        }
    }
    
    private static async Task<Result> ExecuteStream<TStream, T>(TStream stream, Func<TStream, Task> operation)
        where TStream : IDisposable
    {
        try
        {
            await operation(stream);
            return Result.Success();
        }
        catch (FileNotFoundException e)
        {
            return Result.Failure(new Error(e.HResult.ToString(),"The file or directory cannot be found."));
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("The file or directory cannot be found.");
            return Result.Failure(new Error(e.HResult.ToString(),"The directory cannot be found."));
        }
        catch (DriveNotFoundException e)
        {
            Console.WriteLine("The drive specified in 'path' is invalid.");
            return Result.Failure(new Error(e.HResult.ToString(),"The drive specified in 'path' is invalid."));
        }
        catch (PathTooLongException e)
        {
            Console.WriteLine("'path' exceeds the maximum supported path length.");
            return Result.Failure(new Error(e.HResult.ToString(),"The path is too long."));
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("You do not have permission to create this file.");
            return Result.Failure(new Error(e.HResult.ToString(),"You do not have permission to create this file."));
        }
        catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
        {
            Console.WriteLine("There is a sharing violation.");
            return Result.Failure(new Error(e.HResult.ToString(),"There is a sharing violation."));
        }
        catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
        {
            Console.WriteLine("The file already exists.");
            return Result.Failure(new Error(e.HResult.ToString(),"The file already exists."));
        }
        catch (IOException e)
        {
            return Result.Failure(new Error(e.HResult.ToString(),e.Message));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Failure(new Error(e.HResult.ToString(),e.Message));
        }
    }
}