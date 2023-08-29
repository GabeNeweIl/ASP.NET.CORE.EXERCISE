var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (HttpContext context) =>
{
    if (context.Request.Method == "GET")
    {
        var queryParams = new QueryParams
        {
            FirstNumber = context.Request.Query["firstNumber"],
            SecondNumber = context.Request.Query["secondNumber"],
            Operation = context.Request.Query["operation"]
        };
    }
});

static List<string> GetNullFields(object obj)
{
    if (obj == null)
    {
        throw new ArgumentNullException(nameof(obj));
    }

    var nullFields = new List<string>();

    var type = obj.GetType();
    foreach (var field in type.GetFields())
    {
        var value = field.GetValue(obj);
        if (value == null)
        {
            nullFields.Add(field.Name);
        }
    }

    return nullFields;
}

static IsValidQueryParams IsValid(QueryParams queryParams, HttpContext context)
{
    var result = new IsValidQueryParams
    {
        FieldNames = new List<string?>(),
        IsValid = true
    };

    var isNullFieldsResult = GetNullFields(queryParams);

    if (isNullFieldsResult is not null)
    {
        result.FieldNames = isNullFieldsResult;
        result.IsValid = false;

        return result;
    }

    try
    {
        var firstNumber = Convert.ToInt32(queryParams.FirstNumber);
        var secondNumber = Convert.ToInt32(queryParams.SecondNumber);
    }
    catch (FormatException ex)
    {
        
    }

    return result;
}

public class QueryParams
{
    public string? FirstNumber { get; set; }

    public string? SecondNumber { get; set; }

    public string? Operation { get; set; }
}

public class IsValidQueryParams
{
    public List<string?> FieldNames { get; set; }

    public bool IsValid { get; set; }
}