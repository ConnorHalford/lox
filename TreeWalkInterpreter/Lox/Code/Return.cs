public class Return : System.Exception
{
	public object Value = null;

	public Return(object value)
	{
		Value = value;
	}
}
