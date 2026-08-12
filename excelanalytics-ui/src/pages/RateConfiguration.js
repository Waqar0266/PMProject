import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../services/api";

export default function RateConfiguration() {

    const [countries, setCountries] = useState([]);
    const [projects, setProjects] = useState([]);
    const [rates, setRates] = useState([]);
    const [currencies, setCurrencies] = useState([]);

    const [countryId, setCountryId] = useState("");
    const [projectId, setProjectId] = useState("");
    const [currencyId, setCurrencyId] = useState("");
    const [year, setYear] = useState(new Date().getFullYear());
    const [month, setMonth] = useState(new Date().getMonth() + 1);
    const [rate, setRate] = useState("");
    const [search, setSearch] = useState("");
    const [editingId, setEditingId] = useState(null);

    useEffect(() => {
        loadCountries();
        loadProjects();
        loadCurrencies();
        loadRates();
    }, []);

    const loadCountries = () => {
        api.get("/Countries")
            .then(res => setCountries(res.data))
            .catch(err => console.log(err));
    };

    const loadProjects = () => {
        api.get("/Projects")
            .then(res => setProjects(res.data))
            .catch(err => console.log(err));
    };

    const loadCurrencies = () => {
        api.get("/Currency")
            .then(res => setCurrencies(res.data))
            .catch(err => console.log(err));
    };

    const loadRates = () => {
        api.get("/RateConfigurations")
            .then(res => setRates(res.data))
            .catch(err => console.log(err));
    };

    const resetForm = () => {
        setEditingId(null);
        setCountryId("");
        setProjectId("");
        setCurrencyId("");
        setRate("");
        setYear(new Date().getFullYear());
        setMonth(new Date().getMonth() + 1);
    };

    const saveRate = () => {
        if (countryId === "") {
            alert("Please select country.");
            return;
        }
        if (projectId === "") {
            alert("Please select project.");
            return;
        }
        if (currencyId === "") {
            alert("Please select currency.");
            return;
        }
        if (rate === "") {
            alert("Please enter rate.");
            return;
        }

        api.post("/RateConfigurations", {
            countryId,
            projectId,
            currencyId,
            year: Number(year),
            month: Number(month),
            rate: Number(rate)
        })
            .then(() => {
                alert("Rate saved successfully.");
                resetForm();
                loadRates();
            })
            .catch(err => {
                console.log(err);
                alert("Unable to save rate.");
            });
    };

    const editRate = (item) => {
        setEditingId(item.id);
        setCountryId(item.countryId);
        setProjectId(item.projectId);
        setCurrencyId(item.currencyId);
        setYear(item.year);
        setMonth(item.month);
        setRate(item.rate);
    };

    const updateRate = () => {
        api.put(`/RateConfigurations/${editingId}`, {
            countryId,
            projectId,
            currencyId,
            year: Number(year),
            month: Number(month),
            rate: Number(rate),
            isActive: true
        })
            .then(() => {
                alert("Rate updated successfully.");
                resetForm();
                loadRates();
            })
            .catch(err => {
                console.log(err);
                alert("Update failed.");
            });
    };

    const filteredRates = rates.filter(x =>
        x.countryName.toLowerCase().includes(search.toLowerCase()) ||
        x.projectName.toLowerCase().includes(search.toLowerCase()) ||
        x.currencyCode.toLowerCase().includes(search.toLowerCase()) ||
        x.currencyName.toLowerCase().includes(search.toLowerCase())
    );

    const deleteRate = (id) => {
        if (!window.confirm("Delete this rate?"))
            return;

        api.delete(`/RateConfigurations/${id}`)
            .then(() => {
                alert("Deleted successfully.");
                loadRates();
            })
            .catch(err => {
                console.log(err);
                alert("Delete failed.");
            });
    };

    const monthNames = [
        "", "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];

    return (
        <div className="container-fluid p-4">
            <div className="card shadow border-0">
                <div className="card-header bg-white d-flex justify-content-between align-items-center">
                    <div>
                        <h3 className="mb-0">💰 Rate Configuration</h3>
                        <small className="text-muted">
                            Rates are applied per country + project when Excel is uploaded.
                        </small>
                    </div>
                    <Link to="/project" className="btn btn-outline-primary btn-sm">
                        Manage Projects →
                    </Link>
                </div>

                <div className="card-body">
                    <div className="row g-3">
                        <div className="col-md-3">
                            <label className="form-label fw-semibold">Country</label>
                            <select
                                className="form-select"
                                value={countryId}
                                onChange={(e) => setCountryId(e.target.value)}
                            >
                                <option value="">Select Country</option>
                                {countries.map(country => (
                                    <option key={country.id} value={country.id}>
                                        {country.name}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="col-md-3">
                            <label className="form-label fw-semibold">Project</label>
                            <select
                                className="form-select"
                                value={projectId}
                                onChange={(e) => setProjectId(e.target.value)}
                            >
                                <option value="">Select Project</option>
                                {projects.filter(p => p.isActive).map(project => (
                                    <option key={project.id} value={project.id}>
                                        {project.name}
                                    </option>
                                ))}
                            </select>
                            {projects.filter(p => p.isActive).length === 0 && (
                                <small className="text-danger">
                                    No active projects.{" "}
                                    <Link to="/project">Add a project first</Link>
                                </small>
                            )}
                        </div>

                        <div className="col-md-2">
                            <label className="form-label fw-semibold">Currency</label>
                            <select
                                className="form-select"
                                value={currencyId}
                                onChange={(e) => setCurrencyId(e.target.value)}
                            >
                                <option value="">Select Currency</option>
                                {currencies.map(currency => (
                                    <option key={currency.id} value={currency.id}>
                                        {currency.code} ({currency.symbol})
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="col-md-2">
                            <label className="form-label fw-semibold">Year</label>
                            <input
                                type="number"
                                className="form-control"
                                value={year}
                                onChange={(e) => setYear(e.target.value)}
                            />
                        </div>

                        <div className="col-md-2">
                            <label className="form-label fw-semibold">Month</label>
                            <select
                                className="form-select"
                                value={month}
                                onChange={(e) => setMonth(e.target.value)}
                            >
                                {monthNames.slice(1).map((name, index) => (
                                    <option key={name} value={index + 1}>{name}</option>
                                ))}
                            </select>
                        </div>

                        <div className="col-md-2">
                            <label className="form-label fw-semibold">Rate</label>
                            <input
                                type="number"
                                className="form-control"
                                placeholder="Enter Rate"
                                value={rate}
                                onChange={(e) => setRate(e.target.value)}
                            />
                        </div>
                    </div>

                    <div className="row mt-4">
                        <div className="col-12 text-end">
                            <button
                                className={`btn ${editingId ? "btn-warning" : "btn-success"} px-5`}
                                onClick={editingId ? updateRate : saveRate}
                            >
                                {editingId ? "Update Rate" : "Save Rate"}
                            </button>
                        </div>
                    </div>

                    <div className="row mb-3 mt-4">
                        <div className="col-md-4">
                            <input
                                className="form-control"
                                placeholder="🔍 Search country, project or currency..."
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </div>
                        <div className="col-md-8 text-end">
                            <span className="badge bg-primary fs-6">
                                {filteredRates.length} Records
                            </span>
                        </div>
                    </div>

                    <hr />

                    <table className="table table-hover">
                        <thead className="table-dark">
                            <tr>
                                <th>Country</th>
                                <th>Project</th>
                                <th>Currency</th>
                                <th>Year</th>
                                <th>Month</th>
                                <th>Rate</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredRates.map(item => (
                                <tr key={item.id}>
                                    <td>{item.countryName}</td>
                                    <td>{item.projectName}</td>
                                    <td>{item.currencySymbol} {item.currencyCode}</td>
                                    <td>{item.year}</td>
                                    <td>{monthNames[item.month]}</td>
                                    <td className="fw-bold text-success">
                                        {item.currencySymbol} {Number(item.rate).toLocaleString()}
                                    </td>
                                    <td>
                                        <button
                                            className="btn btn-warning btn-sm me-2"
                                            onClick={() => editRate(item)}
                                        >
                                            Edit
                                        </button>
                                        <button
                                            className="btn btn-danger btn-sm"
                                            onClick={() => deleteRate(item.id)}
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
