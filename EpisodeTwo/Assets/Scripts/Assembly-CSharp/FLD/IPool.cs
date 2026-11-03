namespace FLD
{
	public interface IPool<T>
	{
		int Count { get; }

		bool IsEmpty { get; }

		bool CanTake { get; }

		T Take();

		void Recycle(T pObject);
	}
}
