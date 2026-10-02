namespace WebSystemTools.SwaggerTools;

//ავთენტიფიკაციის სქემა, რომელსაც Swagger-ის დოკუმენტი აცხადებს
public enum ESwaggerSecurityScheme
{
    //სქემა არ ცხადდება
    None,

    //JWT ტოკენი Authorization header-ში (Bearer)
    JwtBearer,

    //API გასაღები query პარამეტრში ApiKey, როგორც მას WebSystemTools.ApiKeyIdentity კითხულობს
    ApiKey
}
