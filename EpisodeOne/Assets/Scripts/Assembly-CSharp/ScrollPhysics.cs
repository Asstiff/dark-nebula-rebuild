public class ScrollPhysics
{
	public float spring = 10.5f;

	public float drag = 0.3f;

	public float position;

	public float velocity;

	public void Update(float time)
	{
		velocity *= 1f - drag * time;
		position += velocity * time;
	}

	public void SpringToTarget(float targetPosition, float time)
	{
		velocity += (targetPosition - position) * spring * time;
	}
}
