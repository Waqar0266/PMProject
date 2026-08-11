import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
} from "chart.js";

import { Bar } from "react-chartjs-2";

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

export default function CountryChart({ employees }) {

    const grouped = {};

    employees.forEach(emp => {

        const country = emp.allocatedFor || "Unknown";

        if (!grouped[country])
            grouped[country] = 0;

        grouped[country] += emp.revenue;
    });

    const data = {
        labels: Object.keys(grouped),
        datasets: [
            {
                label: "Revenue",
                data: Object.values(grouped)
            }
        ]
    };

    return (
        <div className="card shadow mt-4">
            <div className="card-header">
                <h5>Revenue by Country</h5>
            </div>

            <div className="card-body">
                <Bar data={data} />
            </div>
        </div>
    );
}