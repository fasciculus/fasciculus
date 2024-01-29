import { Vector } from "./Common";
import { Rooms } from "./Rooms";

export class Extension
{
    readonly extension: StructureExtension;

    constructor(extension: StructureExtension)
    {
        this.extension = extension;
    }
}

export class Extensions
{
    private static _my: Vector<Extension> = new Vector();

    static get my(): Vector<Extension> { return Extensions._my.clone(); }

    static initialize()
    {
        Extensions._my = Rooms.myExtensions.map(e => new Extension(e));
    }
}