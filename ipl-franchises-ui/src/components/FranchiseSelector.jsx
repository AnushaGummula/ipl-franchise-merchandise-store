const franchises = [
  {
    code: 'CSK',
    name: 'Chennai Super Kings',
    logo: '/images/teams/CSK_Logo.png',
  },
  {
    code: 'MI',
    name: 'Mumbai Indians',
    logo: '/images/teams/MI_Logo.png',
  },
  {
    code: 'RCB',
    name: 'Royal Challengers Bengaluru',
    logo: '/images/teams/RCB_Logo.png',
  },
  {
    code: 'KKR',
    name: 'Kolkata Knight Riders',
    logo: '/images/teams/KKR_Logo.png',
  },
  {
    code: 'SRH',
    name: 'Sunrisers Hyderabad',
    logo: '/images/teams/SRH_Logo.png',
  },
  {
    code: 'RR',
    name: 'Rajasthan Royals',
    logo: '/images/teams/RR_Logo.png',
  },
  {
    code: 'DC',
    name: 'Delhi Capitals',
    logo: '/images/teams/DC_Logo.png',
  },
  {
    code: 'PBKS',
    name: 'Punjab Kings',
    logo: '/images/teams/PBKS_Logo.png',
  },
  {
    code: 'GT',
    name: 'Gujarat Titans',
    logo: '/images/teams/GT_Logo.png',
  },
  {
    code: 'LSG',
    name: 'Lucknow Super Giants',
    logo: '/images/teams/LSG_Logo.png',
  },
]

function FranchiseSelector({ selectedFranchise, onSelectFranchise }) {
  return (
    <div className="franchise-selector">
      {franchises.map(franchise => (
        <button
          key={franchise.code}
          type="button"
          className={
            selectedFranchise === franchise.code ? 'franchise-button active' : 'franchise-button'
          }
          onClick={() => onSelectFranchise(franchise.code)}
          aria-label={`Shop ${franchise.name} merchandise`}
        >
          <div className="team-logo-image-wrap">
            <img src={franchise.logo} alt={`${franchise.name} logo`} className="team-logo-image" />
          </div>

          <strong>{franchise.code}</strong>
        </button>
      ))}
    </div>
  )
}

export default FranchiseSelector
