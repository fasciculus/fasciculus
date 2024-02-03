import { Objects } from "./types.common";

declare global
{
    interface Game
    {
        hello(): void;
    }

    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
        sub<T>(root: string, key: string, initial: T): T;
    }
}

export class Screeps
{
    static setup()
    {
        Objects.setFunction(Game, "hello", function (): void
        {
            console.log("Game says hello.");
        });

        Objects.setFunction(Memory, "get", function <T>(key: string, initial: T): T
        {
            var result: any | undefined = Memory[key];

            if (!result)
            {
                Memory[key] = result = initial;
            }

            return result as T;
        });

        Objects.setFunction(Memory, "sub", function <T>(root: string, key: string, initial: T): T
        {
            const parent: { [index: string]: T } = Memory.get(root, {});
            var result: T | undefined = parent[key];

            if (!result)
            {
                parent[key] = result = initial;
            }

            return result as T;
        });
    }
}

export { };
