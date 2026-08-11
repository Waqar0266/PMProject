import { useEffect, useState } from "react";
import api from "../services/api";

export default function CountryMaster() {

    const [countries, setCountries] = useState([]);

    useEffect(() => {
        loadCountries();
    }, []);

    const loadCountries = () => {

        api.get("/Countries")
            .then(res => setCountries(res.data))
            .catch(err => console.log(err));

    };

    return (

        <div className="container-fluid p-4">

            <div className="card border-0 shadow-sm">

                <div className="card-header bg-white d-flex justify-content-between">

                    <h4>🌍 Country Master</h4>

                    <button className="btn btn-primary">
                        + Add Country
                    </button>

                </div>

                <div className="card-body">

                    <table className="table table-hover">

                        <thead className="table-light">

                            <tr>

                                <th>Code</th>
                                <th>Name</th>
                                <th>Currency</th>
                                <th>Status</th>
                                <th>Actions</th>

                            </tr>

                        </thead>

                        <tbody>

                            {countries.map(country => (

                                <tr key={country.id}>

                                    <td>{country.code}</td>

                                    <td>{country.name}</td>

                                    <td>{country.currencyCode}</td>

                                    <td>

                                        <span
                                            className={
                                                country.isActive
                                                    ? "badge bg-success"
                                                    : "badge bg-danger"
                                            }
                                        >
                                            {country.isActive
                                                ? "Active"
                                                : "Inactive"}

                                        </span>

                                    </td>

                                    <td>

                                        <button className="btn btn-warning btn-sm me-2">
                                            Edit
                                        </button>

                                        <button className="btn btn-danger btn-sm">
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