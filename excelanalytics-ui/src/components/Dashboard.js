import { useEffect, useState } from "react";
import api from "../services/api";
import UploadExcel from "./UploadExcel";
import CountryChart from "./CountryChart";
import ExportExcel from "./ExportExcel";
export default function Dashboard() {

    const [summary, setSummary] = useState({});
    const [employees, setEmployees] = useState([]);

    const [countryFilter, setCountryFilter] = useState("");
    const [projectFilter, setProjectFilter] = useState("");
    const [search, setSearch] = useState("");
const [yearFilter, setYearFilter] = useState("");
const [monthFilter, setMonthFilter] = useState("");
    useEffect(() => {
        loadDashboard();
    }, []);

    const loadDashboard = () => {

        api.get("/Dashboard/summary")
            .then(res => setSummary(res.data));

        api.get("/Dashboard")
            .then(res => setEmployees(res.data));
    };

const filteredEmployees = employees.filter(emp => {

    const countryMatch =
        countryFilter === "" ||
        emp.allocatedFor === countryFilter;

    const projectMatch =
        projectFilter === "" ||
        emp.projectName === projectFilter;

    const yearMatch =
        yearFilter === "" ||
        emp.year === Number(yearFilter);

    const monthMatch =
        monthFilter === "" ||
        emp.month === Number(monthFilter);

    const searchMatch =
        search === "" ||
        emp.employeeName?.toLowerCase().includes(search.toLowerCase()) ||
        emp.employeeCode?.toLowerCase().includes(search.toLowerCase());

    return (
        countryMatch &&
        projectMatch &&
        yearMatch &&
        monthMatch &&
        searchMatch
    );

});

    return (

        <div
            className="container-fluid py-4"
            style={{
                background: "#F5F7FB",
                minHeight: "100vh"
            }}
        >

            <div className="d-flex justify-content-between align-items-center mb-4">

                <h2
                    style={{
                        fontWeight: "700",
                        color: "#1F2937"
                    }}
                >
                    📊 PMO Analytics Dashboard
                </h2>

                <UploadExcel onUploadSuccess={loadDashboard} />

            </div>

            <div className="row g-3 mb-4">

                <div className="col-md-3">

                    <div
                        className="card border-0 shadow-sm h-100"
                        style={{
                            borderRadius: "15px",
                            borderLeft: "5px solid #4F46E5"
                        }}
                    >
                        <div className="card-body">

                            <small className="text-muted fw-bold">
                                Total Employees
                            </small>

                            <h2 className="mt-2 fw-bold">
                                {summary.totalEmployees}
                            </h2>

                        </div>

                    </div>

                </div>

                <div className="col-md-3">

                    <div
                        className="card border-0 shadow-sm h-100"
                        style={{
                            borderRadius: "15px",
                            borderLeft: "5px solid #10B981"
                        }}
                    >
                        <div className="card-body">

                            <small className="text-muted fw-bold">
                                Total Projects
                            </small>

                            <h2 className="mt-2 fw-bold">
                                {summary.totalProjects}
                            </h2>

                        </div>

                    </div>

                </div>

                <div className="col-md-3">

                    <div
                        className="card border-0 shadow-sm h-100"
                        style={{
                            borderRadius: "15px",
                            borderLeft: "5px solid #F59E0B"
                        }}
                    >
                        <div className="card-body">

                            <small className="text-muted fw-bold">
                                Total Countries
                            </small>

                            <h2 className="mt-2 fw-bold">
                                {summary.totalCountries}
                            </h2>

                        </div>

                    </div>

                </div>

                <div className="col-md-3">

                    <div
                        className="card border-0 shadow-sm h-100"
                        style={{
                            borderRadius: "15px",
                            borderLeft: "5px solid #EF4444"
                        }}
                    >
                        <div className="card-body">

                            <small className="text-muted fw-bold">
                                Total Revenue
                            </small>

                          <h2 className="mt-2 fw-bold text-success">
    $
    {Number(summary.totalRevenue ?? 0).toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    })}
</h2>

                        </div>

                    </div>

                </div>

            </div>

            <div
                className="card border-0 shadow-sm"
                style={{
                    borderRadius: "15px"
                }}
            >

                <div className="card-body">

                  <div className="d-flex justify-content-between align-items-center mb-3"></div>

                    <div className="row g-2 mb-4">

                        <div className="col-md-5">

                            <input
                                type="text"
                                className="form-control"
                                placeholder="🔍 Search Employee..."
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />

                        </div>

                        <div className="col-md-3">

                            <select
                                className="form-select"
                                value={countryFilter}
                                onChange={(e) => setCountryFilter(e.target.value)}
                            >

                                <option value="">🌍 All Countries</option>

                                {[...new Set(employees.map(x => x.allocatedFor))]
                                    .map(country => (

                                        <option
                                            key={country}
                                            value={country}
                                        >
                                            {country}
                                        </option>

                                    ))}

                            </select>

                        </div>

                        <div className="col-md-4">

                            <select
                                className="form-select"
                                value={projectFilter}
                                onChange={(e) => setProjectFilter(e.target.value)}
                            >

                                <option value="">📁 All Projects</option>

                                {[...new Set(employees.map(x => x.projectName))]
                                    .map(project => (

                                        <option
                                            key={project}
                                            value={project}
                                        >
                                            {project}
                                        </option>

                                    ))}

                            </select>

                        </div>
                        <div className="col-md-2">

    <select
        className="form-select"
        value={yearFilter}
        onChange={(e) => setYearFilter(e.target.value)}
    >
        <option value="">📅 All Years</option>

        {[...new Set(employees.map(x => x.year))]
            .sort((a, b) => b - a)
            .map(year => (
                <option key={year} value={year}>
                    {year}
                </option>
            ))}
    </select>

</div>
<div className="col-md-2">

    <select
        className="form-select"
        value={monthFilter}
        onChange={(e) => setMonthFilter(e.target.value)}
    >
        <option value="">🗓 All Months</option>

        <option value="1">January</option>
        <option value="2">February</option>
        <option value="3">March</option>
        <option value="4">April</option>
        <option value="5">May</option>
        <option value="6">June</option>
        <option value="7">July</option>
        <option value="8">August</option>
        <option value="9">September</option>
        <option value="10">October</option>
        <option value="11">November</option>
        <option value="12">December</option>

    </select>

</div>

                    </div>

                    <table className="table table-striped table-hover align-middle">

                        <thead className="table-light">

                            <tr>

                                <th>Employee</th>
                                <th>Project</th>
                                <th>Country</th>
                                <th>Month</th>
<th>Year</th>
                                <th>Grade</th>
                                <th>Allocation Days</th>
                              <th>Revenue (USD)</th>

                            </tr>

                        </thead>

                        <tbody>

                            {filteredEmployees.map((emp) => (

                                <tr key={emp.id}>

                                    <td>{emp.employeeName}</td>

                                    <td>{emp.projectName}</td>

                                    <td>{emp.allocatedFor}</td>
<td>
    {new Date(2000, emp.month - 1).toLocaleString("default", {
        month: "long"
    })}
</td>

<td>{emp.year}</td>
                                    <td>{emp.grade}</td>

                                    <td>{emp.finalAllocationDays}</td>

                                    <td className="fw-bold text-success">
    $
    {Number(emp.revenueUSD ?? 0).toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    })}
</td>

                                </tr>

                            ))}

                        </tbody>

                    </table>

                </div>

            </div>

            <div className="mt-4">

                <CountryChart employees={filteredEmployees} />

            </div>

        </div>

    );

}