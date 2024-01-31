import { ControllerId, Dictionaries, Dictionary, GameWrap, Vector } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";

export class Controller
{
    readonly id: ControllerId;

    get controller(): StructureController { return GameWrap.get<StructureController>(this.id)!; }

    get my(): boolean { return this.controller.my; }
    get pos(): RoomPosition { return this.controller.pos; }

    constructor(id: ControllerId)
    {
        this.id = id;
    }
}

export class Controllers
{
    private static _allControllers: Dictionary<Controller> = {};

    static get(id: ControllerId | undefined): Controller | undefined { return id ? Controllers._allControllers[id] : undefined; }

    static get all(): Vector<Controller> { return Dictionaries.values(Controllers._allControllers); }
    static get my(): Vector<Controller> { return Controllers.all.filter(c => c.my); }
    static get myCount(): number { return Controllers.my.length; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Controllers._allControllers = {};
        }

        Dictionaries.update(Controllers._allControllers, Chambers.allControllers, id => new Controller(id as ControllerId));
    }
}