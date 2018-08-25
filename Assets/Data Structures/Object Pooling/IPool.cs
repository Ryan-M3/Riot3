public interface IPool {
    Poolable Borrow();
    void Return(Poolable p);
}