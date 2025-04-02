function ResultsDisplay({ fCamara, competitor }) {
    return (
      <div>
        <h3>Results</h3>
        <p>Total FCamara commission: <span>£{fCamara}</span></p>
        <p>Total Competitor commission: <span>£{competitor}</span></p>
      </div>
    );
  }
  
  export default ResultsDisplay;
  