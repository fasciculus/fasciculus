import { Bodies } from "./Bodies";
import { Builders } from "./Builders";
import { CreepBase, CreepState, CreepType, Creeps, ICreepMemory } from "./Creeps";
import { Extensions } from "./Extensions";
import { Customer, IdCustomer, IdSupply, Supply } from "./Objects";
import { Spawns } from "./Spawns";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";

const MIN_SUPPLIER_ENERGY = 1;
const MIN_SUPPLIER_CAPACITY = 1;
const MIN_SUPPLY_ENERGY = 10;

const SUPPLIER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

export class Supplier extends CreepBase
{
    constructor(creep: Creep)
    {
        super(creep);
    }

    run()
    {
        var state = this.prepare(this.state, this.supply, this.customer);

        switch (state)
        {
            case CreepState.MoveToSupply: this.moveTo(this.supply!, SUPPLIER_MOVE_TO_OPTS); break;
            case CreepState.MoveToCustomer: this.moveTo(this.customer!, SUPPLIER_MOVE_TO_OPTS); break;
            case CreepState.Withdraw: this.withdraw(this.supply!, RESOURCE_ENERGY); break;
            case CreepState.Transfer: this.transfer(this.customer!, RESOURCE_ENERGY); break;
        }

        this.state = state;
    }

    private prepare(state: CreepState, supply?: Supply, customer?: Customer): CreepState
    {
        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle(supply, customer);
            case CreepState.MoveToSupply: return this.prepareMoveToSupply(supply, customer);
            case CreepState.MoveToCustomer: return this.prepareMoveToCustomer(supply, customer);
            case CreepState.Withdraw: return this.prepareWithdraw(supply, customer);
            case CreepState.Transfer: return this.prepareTransfer(supply, customer);
        }

        return state;
    }

    private prepareIdle(supply?: Supply, customer?: Customer): CreepState
    {
        if (this.energy < MIN_SUPPLIER_ENERGY)
        {
            this.customer = undefined;

            if (!supply)
            {
                this.supply = supply = this.findSupply();
            }

            return supply ? (this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.MoveToSupply) : CreepState.Idle;
        }
        else
        {
            this.supply = undefined;

            if (!customer)
            {
                this.customer = customer = this.findCustomer();
            }

            return customer ? (this.inRangeTo(customer) ? CreepState.Transfer : CreepState.MoveToCustomer) : CreepState.Idle;
        }
    }

    private prepareMoveToSupply(supply?: Supply, customer?: Customer): CreepState
    {
        if (!supply) return this.prepareIdle(supply, customer);

        if (!Supplier.hasEnergy(supply))
        {
            this.supply = supply = undefined;
            return this.prepareIdle(supply, customer);
        }

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.MoveToSupply;
    }

    private prepareMoveToCustomer(supply?: Supply, customer?: Customer): CreepState
    {
        if (!customer) return this.prepareIdle(supply, customer);

        if (!Supplier.hasCapacity(customer))
        {
            this.customer = customer = undefined;
            return this.prepareIdle(supply, customer);
        }

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.MoveToCustomer;
    }

    private prepareWithdraw(supply?: Supply, customer?: Customer): CreepState
    {
        if (!supply) return this.prepareIdle(supply, customer);
        if (this.freeEnergyCapacity < MIN_SUPPLIER_CAPACITY) return this.prepareIdle(supply, customer);
        if (!this.inRangeTo(supply)) return CreepState.MoveToSupply;

        if (!Supplier.hasEnergy(supply))
        {
            this.supply = supply = undefined;
            return this.prepareIdle(supply, customer);
        }

        return CreepState.Withdraw;
    }

    private prepareTransfer(supply?: Supply, customer?: Customer): CreepState
    {
        if (!customer) return this.prepareIdle(supply, customer);
        if (this.energy < MIN_SUPPLIER_ENERGY) return this.prepareIdle(supply, customer);
        if (!this.inRangeTo(customer)) return CreepState.MoveToCustomer;

        if (!Supplier.hasCapacity(customer))
        {
            this.customer = customer = undefined;
            return this.prepareIdle(supply, customer);
        }

        return CreepState.Transfer;
    }

    private inRangeTo(target: Supply | Customer): boolean
    {
        return this.pos.inRangeTo(target, 1);
    }

    private findSupply(): Supply | undefined
    {
        var used = Suppliers.usedSupplies;
        var supplies: Supply[] = Wellers.all.filter(w => !used.has(w.id) && Supplier.hasEnergy(w.creep)).map(w => w.creep);

        if (supplies.length == 0) return undefined;

        return this.pos.findClosestByPath(supplies) || undefined;
    }

    private findCustomer(): Customer | undefined
    {
        var served = Suppliers.servedCustomers;
        var customers: Customer[] = [];

        var spawns: Customer[] = Spawns.my.filter(s => !served.has(s.id)).filter(Supplier.hasCapacity);
        var extensions: Customer[] = Extensions.my.filter(s => !served.has(s.id)).filter(Supplier.hasCapacity);

        customers = spawns.concat(extensions);

        if (customers.length == 0)
        {
            let upgraders = Upgraders.all.filter(u => !served.has(u.id)).map(u => u.creep);
            let builders = Builders.all.filter(b => !served.has(b.id)).map(b => b.creep);

            customers = upgraders.concat(builders);
        }

        if (customers.length == 0) return undefined;

        customers = customers.sort(Supplier.compareCustomers);

        return customers[0];
    }

    private static hasEnergy(supply: Supply)
    {
        return supply.store.energy >= MIN_SUPPLY_ENERGY;
    }

    private static hasCapacity(customer: Customer)
    {
        return customer.store.getFreeCapacity(RESOURCE_ENERGY) > 0;
    }

    private static compareCustomers(a: Customer, b: Customer): number
    {
        var aFree = a.store.getFreeCapacity(RESOURCE_ENERGY);
        var bFree = a.store.getFreeCapacity(RESOURCE_ENERGY);

        return bFree - aFree;
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

    static get usedSupplies(): Set<IdSupply>
    {
        var ids = Suppliers._all.map(s => s.memory.supply).filter(id => id) as IdSupply[];
        var counts: { [id: IdSupply]: number } = {};
        var result: Set<IdSupply> = new Set();

        for (let id of ids)
        {
            let count: number = counts[id] || 0;

            counts[id] = count + 1;
        }

        for (let id of ids)
        {
            if (counts[id] > 1)
            {
                result.add(id);
            }
        }

        return result;
    }

    static get servedCustomers(): Set<IdCustomer>
    {
        var ids = Suppliers._all.map(s => s.memory.customer).filter(id => id) as IdCustomer[];

        return new Set(ids);
    }
}