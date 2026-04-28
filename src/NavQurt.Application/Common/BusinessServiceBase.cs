using NavQurt.Shared;

namespace NavQurt.Application.Common;

internal abstract class BusinessServiceBase
{
    protected static ResponseResult<T> NotFound<T>(string name) => ResponseResult<T>.CreateError($"{name} topilmadi.", WebErrorConstant.NotFound);
    protected static ResponseResult<T> BadRequest<T>(string message) => ResponseResult<T>.CreateError(message, WebErrorConstant.UnknownError);

    protected static bool HasText(string? value) => !string.IsNullOrWhiteSpace(value);
}
