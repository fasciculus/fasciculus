import { Vector, Vectors } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";
import { Stores } from "./Stores";

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

    @profile
    static initialize()
    {
        Extensions._my = Vectors.flatten(Chambers.my.map(c => c.myExtensions)).map(e => new Extension(e));
    }
}