import React from "react";

function CommissionForm({ onSubmit, isLoading }) {
  return (
    <form onSubmit={onSubmit}>
      <label htmlFor="localSalesCount">Local Sales Count</label>
      <input 
        name="localSalesCount" 
        id="localSalesCount"
        type="number" 
        min="0" 
        placeholder="Enter local sales count"
        required
      />
      <br />

      <label htmlFor="foreignSalesCount">Foreign Sales Count</label>
      <input 
        name="foreignSalesCount" 
        id="foreignSalesCount"
        type="number" 
        min="0"
        placeholder="Enter foreign sales count"
      />
      <br />

      <label htmlFor="averageSaleAmount">Average Sale Amount</label>
      <input
        name="averageSaleAmount"
        id="averageSaleAmount"
        type="number"
        step="0.01"
        min="0.01"
        placeholder="Enter average sale amount"
        required
      />
      <br />

      <button type="submit" disabled={isLoading}>
        {isLoading ? "Calculating..." : "Calculate"}
      </button>
    </form>
  );
}

export default CommissionForm;
