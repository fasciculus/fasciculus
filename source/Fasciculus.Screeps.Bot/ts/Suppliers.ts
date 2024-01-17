import { Bodies } from "./Bodies";
import { CreepBase, CreepState, CreepType, Creeps, ICreepMemory } from "./Creeps";
import { Customer, Supply } from "./Objects";
import { Spawns } from "./Spawns";
import { Weller, Wellers } from "./Wellers";

export class Supplier extends CreepBase
{
    constructor(creep: Creep)
    {
        super(creep);
    }

    run()
    {
        var state = this.prepare(this.state);

        switch (state)
        {
            case CreepState.MoveToSupply: this.moveTo(this.supply!); break;
            case CreepState.MoveToCustomer: this.moveTo(this.customer!); break;
            case CreepState.Withdraw: this.withdraw(this.supply!, RESOURCE_ENERGY); break;
        }

        this.state = state;
    }

    private prepare(state: CreepState): CreepState
    {
        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle();
            case CreepState.MoveToSupply: return this.prepareMoveToSupply();
            case CreepState.MoveToCustomer: return this.prepareMoveToCustomer();
            case CreepState.Withdraw: return this.prepareWithdraw();
        }

        return state;
    }

    private prepareIdle(): CreepState
    {
        if (this.energy > 0)
        {
            var customer = this.customer || this.findCustomer();

            this.customer = customer;
            this.supply = undefined;

            return customer ? this.prepare(CreepState.MoveToCustomer) : CreepState.Idle;
        }
        else
        {
            var supply = this.supply || this.findSupply();

            this.customer = undefined;
            this.supply = supply;

            return supply ? this.prepare(CreepState.MoveToSupply) : CreepState.Idle;
        }
    }

    private prepareMoveToSupply(): CreepState
    {
        var supply = this.supply;

        if (!supply) return this.prepareIdle();

        return this.pos.inRangeTo(supply, 1) ? CreepState.Withdraw : CreepState.MoveToSupply;
    }

    private prepareMoveToCustomer(): CreepState
    {
        var customer = this.customer;

        if (!customer) return this.prepareIdle();

        return this.pos.inRangeTo(customer, 1) ? CreepState.Idle : CreepState.MoveToCustomer;
    }

    private prepareWithdraw(): CreepState
    {
        if (this.freeEnergyCapacity < 1) return this.prepareIdle();

        var supply = this.supply;

        if (!supply) return this.prepareIdle();

        if (supply.store.energy < 1)
        {
            this.supply = undefined;
            return this.prepareIdle();
        }

        if (!this.pos.inRangeTo(supply, 1)) return this.prepareMoveToSupply();

        return CreepState.Withdraw;
    }

    private findSupply(): Supply | undefined
    {
        var wellers = Wellers.all.filter(w => w.energy > 49);

        return wellers.length == 0 ? undefined : this.pos.findClosestByPath<Weller>(wellers)?.creep;
    }

    private findCustomer(): Customer | undefined
    {
        var spawns = Spawns.my.filter(s => s.store.getFreeCapacity(RESOURCE_ENERGY) > 0);

        if (spawns.length > 0) return this.pos.findClosestByPath(spawns) || undefined;

        return undefined;
    }
}

const SUPPLIER_PARTS: BodyPartConstant[] = [CARRY, MOVE, CARRY, MOVE, CARRY, MOVE, CARRY, MOVE, CARRY, MOVE];
const SUPPLIER_MIN_SIZE = 2;

export class Suppliers
{
    private static _all: Supplier[] = [];

    static get all(): Supplier[] { return Suppliers._all; }

    static initialize()
    {
        Suppliers._all = Creeps.ofType(CreepType.Supplier).map(c => new Supplier(c));

        Bodies.register(CreepType.Supplier, SUPPLIER_MIN_SIZE, SUPPLIER_PARTS);
    }

    static run()
    {
        Suppliers._all.forEach(s => s.run());
    }
}