import { Rooms } from "./Rooms";
import { Vector } from "./Collections";

export class Extensions
{
    private static _my: Vector<StructureExtension> = new Vector();

    static get my(): Vector<StructureExtension> { return Extensions._my.clone(); }

    static initialize()
    {
        Extensions._my = Rooms.myExtensions;
    }
}