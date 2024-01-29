import { Customer, CustomerPriorities, Vector, Vectors, _Customer } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";
import { Stores } from "./Stores";

export class Extension implements _Customer
{
    readonly extension: StructureExtension;

    readonly customer: Customer;
    readonly priority: number;
    demand: number;

    constructor(extension: StructureExtension)
    {
        this.extension = extension;

        this.customer = extension;
        this.priority = CustomerPriorities["Extension"];
        this.demand = Stores.freeEnergyCapacity(extension);
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