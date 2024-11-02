using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace server.Extentions;


public static class ModelStateExtensions{

    public static object GetErrors(this ModelStateDictionary ModelState){

        return  ModelState
        .Where(e => e.Value?.Errors.Count > 0)
        .Select(e => new
        {
            Field = e.Key,
            Errors = e.Value?.Errors.Select(error => error.ErrorMessage).ToArray()
        })
        .ToArray();

    }


}