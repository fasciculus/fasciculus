
import { Bodies, BodyTemplate, CreepState, CreepType, Customer, CustomerId, GameWrap, Positions, SourceId, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { profile } from "./Profiling";
import { Spawns } from "./Spawns";
import { Stores } from "./Stores";
import { Well, Wells } from "./Wells";

const STARTER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE]);

const STARTER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

interface StarterMemory extends CreepBaseMemory
{
    well?: SourceId;
    customer?: CustomerId;
}

export class Starter extends CreepBase<StarterMemory>
{
    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; }

    get customer(): Customer | undefined { return GameWrap.get(this.memory.customer); }
    set customer(value: Customer | undefined) { this.memory.customer = value?.id; }

    constructor(creep: Creep)
    {
        super(creep, CreepType.Starter);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToWell: this.executeToWell(); break;
            case CreepState.ToCustomer: this.executeToCustomer(); break;
            case CreepState.Harvest: this.executeHarvest(); break;
            case CreepState.Transfer: this.executeTransfer(); break;
        }
    }

    private executeToWell()
    {
        let well = this.well;

        if (well)
        {
            this.moveTo(well);
        }
    }

    private executeToCustomer()
    {
        let customer = this.customer;

        if (!customer) return;

        this.moveTo(customer, STARTER_MOVE_TO_OPTS);
    }

    private executeHarvest()
    {
        let well = this.well;

        if (well)
        {
            this.harvest(well.source);
        }
    }

    private executeTransfer()
    {
        let customer = this.customer;

        if (!customer) return;

        this.transfer(customer, RESOURCE_ENERGY);
    }

    prepare()
    {
        let well = this.well;

        if (well)
        {
            if (this.freeEnergyCapacity > 0 && well.assignable)
            {
                this.state = this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToWell;
            }
            else
            {
                this.well = undefined;
                this.state = CreepState.Idle;
            }

            return;
        }

        let customer = this.customer;

        if (customer)
        {
            if (!Stores.hasFreeEnergyCapacity(customer))
            {
                this.customer = undefined;
                this.state = CreepState.Idle;
            }
            else if (this.energy > 0)
            {
                this.state = this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
            }
            else
            {
                this.customer = undefined;
                this.state = CreepState.Idle;
            }

            return;
        }

        this.state = CreepState.Idle;
    }

    private inRangeTo(target: Well | Customer)
    {
        return this.pos.inRangeTo(target, 1);
    }
}

export class Starters
{
    private static _all: Vector<Starter> = new Vector();

    static get count(): number { return Starters._all.length; }

    static initialize()
    {
        Starters._all = Creeps.ofType(CreepType.Starter).map(c => new Starter(c));

        Bodies.register(CreepType.Starter, STARTER_TEMPLATE);
    }

    @profile
    static run()
    {
        Starters._all.forEach(s => s.prepare());
        Starters.assign().forEach(s => s.prepare());
        Starters._all.forEach(s => s.execute());
    }

    private static assign(): Vector<Starter>
    {
        let unassigned = Starters._all.filter(s => !s.well && !s.customer);
        let harvesters = Starters.assignWells(unassigned);
        let suppliers = Starters.assignCustomers(unassigned);

        return harvesters.concat(suppliers);
    }

    private static assignWells(unassigned: Vector<Starter>): Vector<Starter>
    {
        let result: Vector<Starter> = new Vector();

        unassigned = unassigned.filter(s => s.energy == 0);

        for (let starter of unassigned)
        {
            let wells = Wells.assignable
            let well = Positions.closestByPath(starter, wells);

            if (!well)
            {
                starter.suicide();
            }
            else
            {
                starter.well = well;
                result.append(starter);
            }
        }

        return result;
    }

    private static assignCustomers(unassigned: Vector<Starter>): Vector<Starter>
    {
        let result: Vector<Starter> = new Vector();

        unassigned = unassigned.filter(s => s.energy > 0);

        if (unassigned.length == 0) return result;

        let customers = Starters.findCustomers();

        if (customers.length == 0) return result;

        for (let starter of unassigned)
        {
            let customer = Positions.closestByPath(starter, customers);

            if (!customer) continue;

            starter.customer = customer;
            result.append(starter);
        }

        return result;
    }

    private static findCustomers(): Vector<Customer>
    {
        let spawns: Vector<Customer> = Spawns.my.map(s => s.spawn);
        let extensions: Vector<Customer> = Extensions.my;
        var customers = spawns.concat(extensions);

        customers = customers.filter(c => Stores.hasFreeEnergyCapacity(c));

        return customers;
    }
}