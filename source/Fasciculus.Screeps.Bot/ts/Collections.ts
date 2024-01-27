import { Dictionary, Vector } from "./Common";

export class Dictionaries
{
    static isEmpty<T>(dictionary: Dictionary<T>): boolean
    {
        for (let key in dictionary)
        {
            return false;
        }

        return true;
    }

    static keys<T>(dictionary: Dictionary<T>): Set<string>
    {
        return new Set(Object.keys(dictionary));
    }

    static values<T>(dictionary: Dictionary<T>): Vector<T>
    {
        return new Vector(Object.values(dictionary)); 
    }

    static clone<T>(dictionary: Dictionary<T>): Dictionary<T>
    {
        return Object.assign({}, dictionary);
    }
}

export type PriorityQueueCompare<T> = (a: T, b: T) => number;

interface PriorityQueueEntry<T>
{
    value: T;
    next?: PriorityQueueEntry<T>;
}

export class PriorityQueue<T>
{
    readonly compare: PriorityQueueCompare<T>

    private first?: PriorityQueueEntry<T>;

    constructor(compare: PriorityQueueCompare<T>)
    {
        this.compare = compare;
    }

    push(value: T)
    {
        if (this.first)
        {
            let compare: PriorityQueueCompare<T> = this.compare;

            if (compare(value, this.first.value) < 0)
            {
                this.first = { value, next: this.first };
            }
            else
            {
                let prev: PriorityQueueEntry<T> = this.first;
                let next: PriorityQueueEntry<T> | undefined = prev.next;

                while (next && compare(next.value, value) < 0)
                {
                    prev = next;
                    next = prev.next;
                }

                prev.next = { value, next };
            }
        }
        else
        {
            this.first = { value };
        }
    }

    pop(): T | undefined
    {
        if (this.first)
        {
            let value = this.first.value;

            this.first = this.first.next;

            return value;
        }
        else
        {
            return undefined;
        }
    }
}