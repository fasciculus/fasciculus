import { Rooms } from "./Rooms";
import { Vector } from "./Collections";

export class Sources
{
    private static _all: Vector<Source> = new Vector();

    static get all(): Vector<Source> { return Sources._all.clone(); }

    static initialize()
    {
        Sources._all = Rooms.sources;
    }
}