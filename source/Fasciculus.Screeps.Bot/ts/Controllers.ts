import { Dictionary, Vector, Vectors } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";

export class Controller
{
    readonly controller: StructureController;

    get id(): Id<StructureController> { return this.controller.id; }
    get my(): boolean { return this.controller.my; }
    get pos(): RoomPosition { return this.controller.pos; }

    constructor(controller: StructureController)
    {
        this.controller = controller;
    }
}

export class Controllers
{
    private static _all: Vector<Controller> = new Vector();
    private static _my: Vector<Controller> = new Vector();
    private static _byId: Dictionary<Controller> = {};

    static get(id: Id<StructureController> | undefined): Controller | undefined
    {
        return id ? Controllers._byId[id] : undefined;
    }

    static get my(): Vector<Controller> { return Controllers._my.clone(); }
    static get myCount(): number { return Controllers._my.length; }

    @profile
    static initialize()
    {
        Controllers._all = Vectors.defined(Chambers.all.map(c => c.controller)).map(c => new Controller(c));
        Controllers._my = Controllers._all.filter(c => c.my);
        Controllers._byId = Controllers._all.indexBy(c => c.id);
    }
}