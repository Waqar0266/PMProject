import { BrowserRouter, Routes, Route } from "react-router-dom";

import Dashboard from "./components/Dashboard";
import CountryMaster from "./pages/CountryMaster";
import ProjectMaster from "./pages/ProjectMaster";
import RateConfiguration from "./pages/RateConfiguration";
import Layout from "./Layout";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route index element={<Dashboard />} />
                    <Route path="country" element={<CountryMaster />} />
                    <Route path="project" element={<ProjectMaster />} />
                    <Route path="rate" element={<RateConfiguration />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
