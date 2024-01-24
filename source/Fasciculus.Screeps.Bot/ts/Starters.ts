import * as _ from "lodash";

import { CreepBase, Creeps } from "./Creeps";
import { Bodies, BodyTemplate } from "./Bodies";
import { StarterMemory } from "./Memories";
import { CreepState, CreepType } from "./Enums";
import { Well, Wells } from "./Wells";
import { Customer } from "./Types";
import { GameWrap } from "./GameWrap";
import { Stores } from "./Stores";
import { Spawns } from "./Spawns";
import { Extensions } from "./Extensions";

const STARTER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE]);

const STARTER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

export class Starter extends CreepBase
{
    get memory(): StarterMemory { return super.memory as StarterMemory; }

    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; }

    get customer(): Customer | undefined { return GameWrap.get(this.memory.customer); }
    set customer(value: Customer | undefined) { this.memory.customer = value?.id; }

    constructor(creep: Creep)
    {
        super(creep);
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
    private static _all: Starter[] = [];

    static get count(): number { return Starters._all.length; }
    // static get all(): Starter[] { return Starters._all; }

    static initialize()
    {
        Starters._all = Creeps.ofType(CreepType.Starter).map(c => new Starter(c)).values;

        Bodies.register(CreepType.Starter, STARTER_TEMPLATE);
    }

    static run()
    {
        Starters._all.forEach(s => s.prepare());

        let assigned = Starters.assign();

        assigned.forEach(s => s.prepare());
        Starters._all.forEach(s => s.execute());
    }

    private static assign(): Starter[]
    {
        let unassigned = Starters._all.filter(s => !s.well && !s.customer);
        let harvesters = Starters.assignWells(unassigned);
        let suppliers = Starters.assignCustomers(unassigned);

        return harvesters.concat(suppliers);
    }

    private static assignWells(unassigned: Starter[]): Starter[]
    {
        var result: Starter[] = [];

        unassigned = unassigned.filter(s => s.energy == 0);

        for (let starter of unassigned)
        {
            let wells = Wells.assignable
            let well = starter.pos.findClosestByPath(wells) || undefined;

            if (!well)
            {
                starter.suicide();
            }
            else
            {
                starter.well = well;
                result.push(starter);
            }
        }

        return result;
    }

    private static assignCustomers(unassigned: Starter[]): Starter[]
    {
        unassigned = unassigned.filter(s => s.energy > 0);

        if (unassigned.length == 0) return [];

        let customers = Starters.findCustomers();

        if (customers.length == 0) return [];

        var result: Starter[] = [];

        for (let i = 0, n = unassigned.length; i < n; ++i)
        {
            let starter = unassigned[i];
            let customer = starter.pos.findClosestByPath(customers) || undefined;

            if (!customer) continue;

            starter.customer = customer;
            result.push(starter);
        }

        return result;
    }

    private static findCustomers(): Customer[]
    {
        let spawns: Customer[] = Spawns.my.map(s => s.spawn).values;
        let extensions: Customer[] = Extensions.my.values;
        var customers = spawns.concat(extensions);

        customers = customers.filter(c => Stores.hasFreeEnergyCapacity(c));

        return customers;
    }
}