const PRODUCT_IMAGES = {
  // CSK
  CSK_Jersey: '/images/products/CSK_Jersey.jpg',
  CSK_Cap: '/images/products/CSK_Cap.jpg',
  CSK_Flag: '/images/products/CSK_Flag.png',
  CSK_AutographedPhoto: '/images/products/CSK_AutographedPhoto.png',

  // MI
  MI_Jersey: '/images/products/MI_Jersey.jpg',
  MI_Cap: '/images/products/MI_Cap.png',
  MI_Flag: '/images/products/MI_Flag.jpg',
  MI_AutographedPhoto: '/images/products/MI_AutographedPhoto.png',

  // RCB
  RCB_Jersey: '/images/products/RCB_Jersey.jpg',
  RCB_Cap: '/images/products/RCB_Cap.jpg',
  RCB_Flag: '/images/products/RCB_Flag.png',
  RCB_AutographedPhoto: '/images/products/RCB_AutographedPhoto.png',

  // KKR
  KKR_Jersey: '/images/products/KKR_Jersey.png',
  KKR_Cap: '/images/products/KKR_Cap.png',
  KKR_Flag: '/images/products/KKR_Flag.png',
  KKR_AutographedPhoto: '/images/products/KKR_AutographedPhoto.png',

  // SRH
  SRH_Jersey: '/images/products/SRH_Jersey.jpg',
  SRH_Cap: '/images/products/SRH_Cap.png',
  SRH_Flag: '/images/products/SRH_Flag.jpg',
  SRH_AutographedPhoto: '/images/products/SRH_AutographedPhoto.png',

  // RR
  RR_Jersey: '/images/products/RR_Jersey.png',
  RR_Cap: '/images/products/RR_Cap.png',
  RR_Flag: '/images/products/RR_Flag.png',
  RR_AutographedPhoto: '/images/products/RR_AutographedPhoto.png',

  // DC
  DC_Jersey: '/images/products/DC_Jersey.png',
  DC_Cap: '/images/products/DC_Cap.png',
  DC_Flag: '/images/products/DC_Flag.png',
  DC_AutographedPhoto: '/images/products/DC_AutographedPhoto.png',

  // PBKS
  PBKS_Jersey: '/images/products/PBKS_Jersey.png',
  PBKS_Cap: '/images/products/PBKS_Cap.png',
  PBKS_Flag: '/images/products/PBKS_Flag.png',
  PBKS_AutographedPhoto: '/images/products/PBKS_AutographedPhoto.png',

  // GT
  GT_Jersey: '/images/products/GT_Jersey.png',
  GT_Cap: '/images/products/GT_Cap.png',
  GT_Flag: '/images/products/GT_Flag.png',
  GT_AutographedPhoto: '/images/products/GT_AutographedPhoto.png',

  // LSG
  LSG_Jersey: '/images/products/LSG_Jersey.png',
  LSG_Cap: '/images/products/LSG_Cap.png',
  LSG_Flag: '/images/products/LSG_Flag.png',
  LSG_AutographedPhoto: '/images/products/LSG_AutographedPhoto.png',
}

export const getProductImage = product => {
  if (!product) {
    return '/images/products/product-placeholder.png'
  }

  const franchise = String(product.franchiseCode || '')
    .trim()
    .toUpperCase()

  const type = String(product.productType || '').trim()

  const key = `${franchise}_${type}`

  return PRODUCT_IMAGES[key] || '/images/products/product-placeholder.png'
}
