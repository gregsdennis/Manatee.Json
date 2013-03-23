namespace Manatee.Json.Helpers
{
	internal interface IObjectCaster
	{
		bool TryCast<T>(object obj, out T result);
		T Cast<T>(object obj);
	}
}