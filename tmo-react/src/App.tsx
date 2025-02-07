import React, {useEffect, useState } from 'react';
//import logo from './logo.svg';
import backendApi from './adapters/backendApi';
import { Utilities as utils } from './utilities';

import './App.css';

function App() {
  const [branches, setBranches] = useState<string[]>([]);
  const [selectedBranch, setSelectedBranch] = useState<string>('');
  const [topSellers, setTopSellers] = useState<any[]>([]);
  
  useEffect(()=>{
    const fetchBranches = async () => {
    try {
      const branches = await backendApi.getBranches();
      setBranches(branches);
    } catch(err) {
      console.error('Error fetching branches:', err);
    }
   }
   fetchBranches();
  },[])

  useEffect(()=>{
    const fetchTopSellers = async () => {
      if (!selectedBranch) return;
      try {
        const topSellers = await backendApi.getTopSellers(selectedBranch);
        setTopSellers(topSellers);
      } catch(err) {
        console.error('Error fetching top sellers:', err);
      }
     }
     fetchTopSellers();
    },[selectedBranch])

  return (
    <div className="App">
      <main>
        <div>
          <label htmlFor="branch-select">Select a branch:</label>
          <select
            id="branch-select"
            value={selectedBranch}
            onChange={e => setSelectedBranch(e.target.value)}
          >
            <option value="">Select Branch</option>
            {branches.map((branch, ndx) => (
              <option key={ndx} value={branch}>
                {branch}
              </option>
            ))}
          </select>
        </div>
        {topSellers.length > 0 && (
          <table>
            <thead>
              <tr>
                <th>Month</th>
                <th>Seller</th>
                <th>Total Orders</th>
                <th>Total Sales</th>
              </tr>
            </thead>
            <tbody>
              {topSellers.map((seller, ndx) => (
                <tr key={ndx}>
                  <td>{seller.month}</td>
                  <td>{seller.seller}</td>
                  <td>{seller.totalOrders}</td>
                  <td>{utils.formatCurrency(seller.totalSales)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </main>
    </div>
  );
}

export default App;
