import { Objects } from "./object";

export class Sets
{
    private static from<T>(iterable?: Iterable<T> | null): Set<T>
    {
        return iterable ? new Set(iterable) : new Set();
    }

    static setup()
    {
        Objects.setFunction(Set, "from", Sets.from);
    }
}