import SalesByDateChart from "../../../components/admin/reports/SalesByDateChart"
import RaceDayTicketsTable from "../../../components/admin/reports/RaceDayTicketsTable"

export default function ReportsPage() {

    const RaceDay = "7037b61e-af59-49ed-aac2-894c655ce917"

    return (
        <div className="flex flex-col gap-10 p-10">

            <h1 className="text-3xl font-bold">
                Reporting Dashboard
            </h1>

            <div className="bg-white p-6 rounded shadow">
                <SalesByDateChart />
            </div>
            <div className="bg-white p-6 rounded shadow">
                <RaceDayTicketsTable raceDayId={RaceDay} />
            </div>

        </div>
    )
}