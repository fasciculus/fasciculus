namespace Fasciculus.Mathematics
{
    public class Matrix<T> : IMatrix<T>
    {
    }

    public class MutableMatrix<T> : Matrix<T>, IMutableMatrix<T>
    {

    }
}
