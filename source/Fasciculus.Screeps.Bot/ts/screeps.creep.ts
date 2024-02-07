import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsCreep
{
    private static type(this: Creep): string
    {
        return this.name.charAt(0);
    }

    private static _my: Cached<Map<string, Creep>> = Cached.simple(ScreepsCreep.fetchMy);

    private static fetchMy(): Map<string, Creep>
    {
        return Objects.values(Game.creeps).indexBy(c => c.name);
    }

    private static my(): Array<Creep>
    {
        return ScreepsCreep._my.value.vs();
    }

    private static myNames(): Set<string>
    {
        return ScreepsCreep._my.value.ks();
    }

    private static _ofType: Cached<Map<string, Array<Creep>>> = Cached.simple(ScreepsCreep.fetchOfType);

    private static fetchOfType(): Map<string, Array<Creep>>
    {
        return ScreepsCreep.my().groupBy(c => c.type);
    }

    private static ofType(type: string): Array<Creep>
    {
        return ScreepsCreep._ofType.value.get(type) || new Array<Creep>();
    }

    private static _namesOfType: Cached<Map<string, Set<string>>> = Cached.simple(ScreepsCreep.fetchNamesOfType);

    private static fetchNamesOfType(): Map<string, Set<string>>
    {
        return ScreepsCreep._ofType.value.map(cs => cs.map(c => c.name).toSet());
    }

    private static namesOfType(type: string): Set<string>
    {
        return ScreepsCreep._namesOfType.value.get(type) || new Set<string>();
    }

    private static get(name: string): Creep | undefined
    {
        return ScreepsCreep._my.value.get(name);
    }

    private static _lastName: Cached<Map<string, number>> = Cached.simple(ScreepsCreep.fetchLastName);

    private static fetchLastName(): Map<string, number>
    {
        const unparsed: Map<string, Array<string>> = ScreepsCreep._ofType.value.map(cs => cs.map(c => c.name.substring(1)));
        const parsed: Map<string, Array<number>> = unparsed.map(ss => ss.map(s => Number.parseInt(s)));

        return parsed.map(vs => vs.max(v => v) || 0);
    }

    private static ensureLastName(key: string): number { return 0; }

    private static newName(type: string)
    {
        const lastNames: Map<string, number> = ScreepsCreep._lastName.value;
        const newName: number = lastNames.ensure(type, ScreepsCreep.ensureLastName) + 1;

        lastNames.set(type, newName);

        return type + newName;
    }

    private static cleanup(): void
    {
        const existing: Set<string> = ScreepsCreep.myNames();

        for (const name in Memory.creeps)
        {
            if (!existing.has(name))
            {
                delete Memory.creeps[name];
            }
        }
    }

    static setup()
    {
        Objects.setGetter(Creep.prototype, "type", ScreepsCreep.type);

        Objects.setGetter(Creep, "my", ScreepsCreep.my);
        Objects.setGetter(Creep, "myNames", ScreepsCreep.myNames);
        Objects.setFunction(Creep, "ofType", ScreepsCreep.ofType);
        Objects.setFunction(Creep, "namesOfType", ScreepsCreep.namesOfType);
        Objects.setFunction(Creep, "get", ScreepsCreep.get);
        Objects.setFunction(Creep, "newName", ScreepsCreep.newName);
        Objects.setFunction(Creep, "cleanup", ScreepsCreep.cleanup);
    }
}