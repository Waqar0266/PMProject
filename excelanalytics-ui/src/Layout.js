import { Outlet, Link } from "react-router-dom";

export default function Layout() {

    return (

        <div className="d-flex">

            <div
                className="bg-dark text-white p-3"
                style={{
                    width: "250px",
                    minHeight: "100vh"
                }}
            >

                <h4 className="mb-4">

                    PMO Analytics

                </h4>

                <Link
                    className="d-block text-white mb-3 text-decoration-none"
                    to="/"
                >
                    📊 Dashboard
                </Link>

                <Link
                    className="d-block text-white mb-3 text-decoration-none"
                    to="/country"
                >
                    🌍 Country Master
                </Link>

                <Link
                    className="d-block text-white text-decoration-none"
                    to="/rate"
                >
                    💲 Rate Configuration
                </Link>

            </div>

            <div
                className="flex-grow-1"
            >

                <Outlet />

            </div>

        </div>

    );

}