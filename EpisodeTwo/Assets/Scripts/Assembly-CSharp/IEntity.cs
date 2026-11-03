using System.Collections.Generic;

public interface IEntity
{
	void SetParameter(string key, string normalValue, string expertValue);

	void SetParameterArray(string memberName, List<AnimatedEntityNode> array);

	void ResetParameters();
}
