import React, { useState } from "react";
import axios from "axios";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./App.css";
import CommissionForm from "./components/CommissionForm"; // Add this
import ResultsDisplay from "./components/ResultsDisplay";
import { useCommissionCalculator } from "./hooks/useCommissionCalculator";

function App() {
  const [totalFcamaraCommission, setTotalFcamaraCommission] = useState(50);
  const [totalCompetitorCommission, setTotalCompetitorCommission] =
    useState(10);

  // Moved calculate function inside component for better scope handling
  const { handleCalculate, isLoading } = useCommissionCalculator(
    setTotalFcamaraCommission,
    setTotalCompetitorCommission
  );

  return (
    <div className="App">
      {/* Properly configured ToastContainer */}
      <ToastContainer
        position="top-center"
        autoClose={2000}
        closeOnClick
        pauseOnHover={false}
        draggable={false}
        hideProgressBar={true}
      />

      <header className="App-header">
        <div></div>
        <CommissionForm onSubmit={handleCalculate} isLoading={isLoading} />
      </header>

      <div>
        <ResultsDisplay
          fCamara={totalFcamaraCommission}
          competitor={totalCompetitorCommission}
        />
      </div>
    </div>
  );
}

export default App;
