using System.Net;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.Run(async (HttpContext context) =>
        {
            if (context.Request.Method == "GET" && context.Request.Path == "/")
            {
                var queryParams = new QueryParams
                {
                    FirstNumber = context.Request.Query["firstNumber"],
                    SecondNumber = context.Request.Query["secondNumber"],
                    Operation = context.Request.Query["operation"]
                };

                var validationResults = Validation.ValidateParameters(queryParams);

                if (validationResults.IsValid)
                {
                    var firstNumber = Convert.ToInt32(queryParams.FirstNumber);
                    var secondNumber = Convert.ToInt32(queryParams.SecondNumber);
                    switch (queryParams.Operation)
                    {
                        case "add":
                            var result = firstNumber + secondNumber;
                            await context.Response.WriteAsync(result.ToString());
                            break;

                        case "subtract":
                            result = firstNumber - secondNumber;
                            await context.Response.WriteAsync(result.ToString());
                            break;

                        case "multiply":
                            result = firstNumber * secondNumber;
                            await context.Response.WriteAsync(result.ToString());
                            break;

                        case "divide":
                            if (Convert.ToInt32(queryParams.SecondNumber) != 0)
                            {
                                result = firstNumber / secondNumber;
                                await context.Response.WriteAsync(result.ToString());
                            }
                            else
                            {
                                context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                                await context.Response.WriteAsync("Dividing by zero");
                            }
                            break;

                        case "exact divide":
                            if (Convert.ToInt32(queryParams.SecondNumber) != 0)
                            {
                                result = firstNumber % secondNumber;
                                await context.Response.WriteAsync(result.ToString());
                            }
                            else
                            {
                                context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                                await context.Response.WriteAsync("Dividing by zero");
                            }
                            break;

                        default:
                            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                            await context.Response.WriteAsync("Invalid input for 'operation'");
                            break;
                    }
                }
                else
                {
                    context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                    var body = string.Empty;

                    foreach (var fieldName in validationResults.FieldNames)
                    {
                        body += $"Invalid input for '{fieldName}'\n";
                    }

                    await context.Response.WriteAsync(body);
                }
            }
            else
            {
                context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                await context.Response.StartAsync();
            }
        });

        app.Run();
    }
}

public class Validation
{
    public static ValidationResults ValidateParameters(QueryParams queryParams)
    {
        var validationResults = new ValidationResults
        {
            FieldNames = new List<string>(),
            IsValid = true
        };

        try
        {
            GetNullFields(queryParams, validationResults.FieldNames);
            CheckTypeInt(queryParams, validationResults.FieldNames);
        }
        catch
        {
            validationResults.IsValid = false;
            return validationResults;
        }

        return validationResults;
    }

    private static void GetNullFields(object obj, List<string> isValidQueryParams)
    {
        var type = obj.GetType();
        var fields = type.GetRuntimeFields();

        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            if (value == null)
            {
                isValidQueryParams.Add(field.Name.Substring(1, field.Name.IndexOf(">") - 1));
            }
        }

        if (isValidQueryParams.Any())
        {
            throw new Exception();
        }
    }

    private static void CheckTypeInt(QueryParams queryParams, List<string> isValidQueryParams)
    {
        try
        {
            Convert.ToInt32(queryParams.FirstNumber);
        }
        catch
        {
            isValidQueryParams.Add(nameof(queryParams.FirstNumber));
        }

        try
        {
            Convert.ToInt32(queryParams.SecondNumber);
        }
        catch
        {
            isValidQueryParams.Add(nameof(queryParams.SecondNumber));
        }

        if (isValidQueryParams.Any())
        {
            throw new Exception();
        }
    }
}

public class QueryParams
{
    public string? FirstNumber { get; set; }

    public string? SecondNumber { get; set; }

    public string? Operation { get; set; }
}

public class ValidationResults
{
    public List<string> FieldNames { get; set; }

    public bool IsValid { get; set; }
}