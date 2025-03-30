import React, { useState } from "react";
import axios from "axios";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import logo from "./logo.svg";
import "./App.css";

// Global state setter functions
let setFCamaraCommissionFunction = null;
let setCompetitorCommissionFunction = null;

// integrate with backend
async function calculate(event) {
  // Prevent default form behavior
  event.preventDefault();

  // Get form data
  const form = event.target;
  const localSalesCount = form.localSalesCount.value;
  const foreignSalesCount = form.foreignSalesCount.value;
  const averageSaleAmount = form.averageSaleAmount.value;

  // Check for empty fields first
  if (!localSalesCount && !foreignSalesCount) {
    toast.error("Please enter at least one sales count");
    return;
  }

  if (!averageSaleAmount) {
    toast.error("Please enter average sale amount");
    return;
  }

  // Convert to numbers for calculation
  const localCount = parseInt(localSalesCount || "0");
  const foreignCount = parseInt(foreignSalesCount || "0");
  const avgAmount = parseFloat(averageSaleAmount || "0");

  // Validation for valid numbers
  if ((localCount === 0 && foreignCount === 0) || avgAmount <= 0) {
    toast.error("Please enter valid sales data");
    return;
  }

  try {
    // Call the API
    const response = await axios.post("https://localhost:5000/commission", {
      localSalesCount: localCount,
      foreignSalesCount: foreignCount,
      averageSaleAmount: avgAmount,
    });

    // Get the results
    const fCamaraAmount = response.data.fCamaraCommissionAmount;
    const competitorAmount = response.data.competitorCommissionAmount;

    // Update state
    setFCamaraCommissionFunction(fCamaraAmount);
    setCompetitorCommissionFunction(competitorAmount);

    // Show success toast
    toast.success("Commission calculated successfully!");
  } catch (error) {
    console.error("Error:", error);
    toast.error("Failed to calculate commission. Please try again.");
  }
}

function App() {
  const [totalFcamaraCommission, setTotalFcamaraCommission] = useState(50);
  const [totalCompetitorCommission, setTotalCompetitorCommission] = useState(10);

  // Set up references to state setter functions
  setFCamaraCommissionFunction = setTotalFcamaraCommission;
  setCompetitorCommissionFunction = setTotalCompetitorCommission;

  return (
    <div className="App">
      <ToastContainer position="top-center" />

      <header className="App-header">
        <div></div>
        <form onSubmit={calculate}>
          <label htmlFor="localSalesCount">Local Sales Count</label>
          <input 
            name="localSalesCount" 
            type="number" 
            min="0" 
            placeholder="Enter local sales count"
            required
          />
          <br />

          <label htmlFor="foreignSalesCount">Foreign Sales Count</label>
          <input 
            name="foreignSalesCount" 
            type="number" 
            min="0"
            placeholder="Enter foreign sales count"
          />
          <br />

          <label htmlFor="averageSaleAmount">Average Sale Amount</label>
          <input
            name="averageSaleAmount"
            type="number"
            step="0.01"
            min="0.01"
            placeholder="Enter average sale amount"
            required
          />
          <br />

          <button type="submit">Calculate</button>
        </form>
      </header>

      <div>
        <h3>Results</h3>
        <p>
          Total FCamara commission: <span>£{totalFcamaraCommission}</span>
        </p>
        <p>
          Total Competitor commission: <span>£{totalCompetitorCommission}</span>
        </p>
      </div>
    </div>
  );
}

export default App;