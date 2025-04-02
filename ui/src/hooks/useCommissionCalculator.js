import { useState } from "react";
import axios from "axios";
import { toast } from "react-toastify";

export function useCommissionCalculator(setFCamara, setCompetitor) {
  const [isLoading, setIsLoading] = useState(false);

  const handleCalculate = async (event) => {
    event.preventDefault();
    setIsLoading(true);

    const form = event.target;
    const localSalesCount = form.localSalesCount.value;
    const foreignSalesCount = form.foreignSalesCount.value;
    const averageSaleAmount = form.averageSaleAmount.value;
    const baseURL = process.env.REACT_APP_API_BASE_URL;

    if (!localSalesCount && !foreignSalesCount) {
      toast.error("Please enter at least one sales count");
      setIsLoading(false);
      return;
    }

    if (!averageSaleAmount) {
      toast.error("Please enter average sale amount");
      setIsLoading(false);
      return;
    }

    const localCount = parseInt(localSalesCount || "0");
    const foreignCount = parseInt(foreignSalesCount || "0");
    const avgAmount = parseFloat(averageSaleAmount || "0");

    if ((localCount === 0 && foreignCount === 0) || avgAmount <= 0) {
      toast.error("Please enter valid sales data");
      setIsLoading(false);
      return;
    }

    try {
      const response = await axios.post(`${baseURL}/commission`, {
        localSalesCount: localCount,
        foreignSalesCount: foreignCount,
        averageSaleAmount: avgAmount,
      });

      setFCamara(response.data.fCamaraCommissionAmount);
      setCompetitor(response.data.competitorCommissionAmount);

      toast.success("Commission calculated successfully!");
    } catch (error) {
      toast.error("Failed to calculate commission. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return { handleCalculate, isLoading };
}
