import { CreepType, Dictionaries, Dictionary, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { profile } from "./Profiling";

interface TankerMemory extends CreepBaseMemory
{
}

export class Tanker extends CreepBase<TankerMemory>
{
    constructor(name: string)
    {
        super(name);
    }
}

export class Tankers
{
    private static _tankers: Dictionary<Tanker> = {};
    private static _all: Vector<Tanker> = new Vector();

    static initialize(reset: boolean)
    {
        if (reset)
        {
            Tankers._tankers = {};
            Tankers._all = new Vector();
        }

        if (Creeps.update(Tankers._tankers, CreepType.Tanker, name => new Tanker(name)))
        {
            Tankers._all = Dictionaries.values(Tankers._tankers);
        }
    }

    static run()
    {
        Tankers.prepare(Tankers._all);
        Tankers.prepare(Tankers.assign());
        Tankers.execute(Tankers._all);
    }

    @profile
    private static prepare(tankers: Vector<Tanker>)
    {
    }

    @profile
    private static execute(tankers: Vector<Tanker>)
    {
    }

    @profile
    private static assign(): Vector<Tanker>
    {
        return new Vector();
    }
}