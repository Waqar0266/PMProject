import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../services/api";

export default function ProjectMaster() {

    const [projects, setProjects] = useState([]);
    const [name, setName] = useState("");
    const [isActive, setIsActive] = useState(true);
    const [editingId, setEditingId] = useState(null);
    const [search, setSearch] = useState("");
    const [message, setMessage] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        loadProjects();
    }, []);

    const loadProjects = () => {
        api.get("/Projects")
            .then(res => setProjects(res.data))
            .catch(err => {
                console.log(err);
                setError("Unable to load projects.");
            });
    };

    const resetForm = () => {
        setEditingId(null);
        setName("");
        setIsActive(true);
        setError(null);
    };

    const saveProject = () => {
        if (!name.trim()) {
            setError("Please enter project name.");
            return;
        }

        setError(null);

        if (editingId) {
            api.put(`/Projects/${editingId}`, {
                name: name.trim(),
                isActive
            })
                .then(() => {
                    setMessage("Project updated successfully.");
                    resetForm();
                    loadProjects();
                })
                .catch(err => {
                    setError(err.response?.data || "Update failed.");
                });
            return;
        }

        api.post("/Projects", { name: name.trim() })
            .then(() => {
                setMessage("Project created successfully.");
                resetForm();
                loadProjects();
            })
            .catch(err => {
                setError(err.response?.data || "Unable to save project.");
            });
    };

    const editProject = (project) => {
        setEditingId(project.id);
        setName(project.name);
        setIsActive(project.isActive);
        setMessage(null);
        setError(null);
    };

    const deleteProject = (project) => {
        if (project.rateConfigurationCount > 0) {
            setError(
                `Cannot delete "${project.name}" — it is linked to ${project.rateConfigurationCount} rate configuration(s). Delete those rates first from Rate Configuration.`
            );
            return;
        }

        if (!window.confirm(`Delete project "${project.name}"?`))
            return;

        setError(null);

        api.delete(`/Projects/${project.id}`)
            .then(() => {
                setMessage("Project deleted successfully.");
                loadProjects();
            })
            .catch(err => {
                setError(err.response?.data || "Delete failed.");
            });
    };

    const filteredProjects = projects.filter(p =>
        p.name.toLowerCase().includes(search.toLowerCase())
    );

    return (
        <div className="container-fluid p-4">
            <div className="card border-0 shadow-sm">
                <div className="card-header bg-white d-flex justify-content-between align-items-center">
                    <div>
                        <h4 className="mb-0">📁 Project Master</h4>
                        <small className="text-muted">
                            Project names must match the Excel <strong>Project Name</strong> column. Projects are used in Rate Configuration.
                        </small>
                    </div>
                    <Link to="/rate" className="btn btn-outline-primary btn-sm">
                        Go to Rate Configuration →
                    </Link>
                </div>

                <div className="card-body">
                    {message && (
                        <div className="alert alert-success py-2">{message}</div>
                    )}
                    {error && (
                        <div className="alert alert-danger py-2">{error}</div>
                    )}

                    <div className="row g-3 mb-4">
                        <div className="col-md-5">
                            <label className="form-label fw-semibold">Project Name</label>
                            <input
                                className="form-control"
                                placeholder="e.g. BHAFC"
                                value={name}
                                onChange={(e) => setName(e.target.value)}
                            />
                        </div>

                        <div className="col-md-3">
                            <label className="form-label fw-semibold">Status</label>
                            <select
                                className="form-select"
                                value={isActive ? "active" : "inactive"}
                                onChange={(e) => setIsActive(e.target.value === "active")}
                                disabled={!editingId}
                            >
                                <option value="active">Active</option>
                                <option value="inactive">Inactive</option>
                            </select>
                            {!editingId && (
                                <small className="text-muted">New projects are active by default.</small>
                            )}
                        </div>

                        <div className="col-md-4 d-flex align-items-end gap-2">
                            <button
                                className={`btn ${editingId ? "btn-warning" : "btn-primary"}`}
                                onClick={saveProject}
                            >
                                {editingId ? "Update Project" : "+ Add Project"}
                            </button>
                            {editingId && (
                                <button
                                    className="btn btn-secondary"
                                    onClick={resetForm}
                                >
                                    Cancel
                                </button>
                            )}
                        </div>
                    </div>

                    <div className="row mb-3">
                        <div className="col-md-4">
                            <input
                                className="form-control"
                                placeholder="🔍 Search projects..."
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </div>
                        <div className="col-md-8 text-end">
                            <span className="badge bg-primary fs-6">
                                {filteredProjects.length} Project(s)
                            </span>
                        </div>
                    </div>

                    <table className="table table-hover align-middle">
                        <thead className="table-light">
                            <tr>
                                <th>Project Name</th>
                                <th>Status</th>
                                <th>Linked Rates</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredProjects.length === 0 && (
                                <tr>
                                    <td colSpan="4" className="text-center text-muted py-4">
                                        No projects found. Add a project to use it in Rate Configuration.
                                    </td>
                                </tr>
                            )}
                            {filteredProjects.map(project => (
                                <tr key={project.id}>
                                    <td className="fw-semibold">{project.name}</td>
                                    <td>
                                        <span className={`badge ${project.isActive ? "bg-success" : "bg-secondary"}`}>
                                            {project.isActive ? "Active" : "Inactive"}
                                        </span>
                                    </td>
                                    <td>
                                        {project.rateConfigurationCount > 0 ? (
                                            <span className="badge bg-info text-dark">
                                                {project.rateConfigurationCount} rate(s)
                                            </span>
                                        ) : (
                                            <span className="text-muted">None</span>
                                        )}
                                    </td>
                                    <td>
                                        <button
                                            className="btn btn-warning btn-sm me-2"
                                            onClick={() => editProject(project)}
                                        >
                                            Edit
                                        </button>
                                        <button
                                            className="btn btn-danger btn-sm"
                                            onClick={() => deleteProject(project)}
                                            disabled={project.rateConfigurationCount > 0}
                                            title={
                                                project.rateConfigurationCount > 0
                                                    ? "Delete linked rates first"
                                                    : "Delete project"
                                            }
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
