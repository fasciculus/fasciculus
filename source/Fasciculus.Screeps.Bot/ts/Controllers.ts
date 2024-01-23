import * as _ from "lodash";
import { Rooms } from "./Rooms";
import { Utils } from "./Utils";

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
    private static _all: Controller[] = [];
    private static _my: Controller[] = [];
    private static _byId: _.Dictionary<Controller>;

    static get(id: Id<StructureController> | undefined): Controller | undefined
    {
        return id ? Controllers._byId[id] : undefined;
    }

    static get all(): Controller[] { return Controllers._all; }
    static get my(): Controller[] { return Controllers._my; }

    static initialize()
    {
        let controllers: StructureController[] = Controllers.defined(Rooms.all.map(r => r.controller).filter(c => c));

        Controllers._all = controllers.map(c => new Controller(c));
        Controllers._my = Controllers._all.filter(c => c.my);
        Controllers._byId = _.indexBy(Controllers._all, c => c.id);
    }

    private static defined(controllers: Array<StructureController | undefined>): StructureController[]
    {
        let result: StructureController[] = [];

        for (let controller of controllers)
        {
            if (controller)
            {
                result.push(controller);
            }
        }

        return result;
    }
}