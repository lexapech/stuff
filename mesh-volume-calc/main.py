import vpython as vp


tris=[  (0,1,2),
            (0,2,3),
            
            (4,5,6),
            (4,6,7),
            	   
            (0,3,6),
            (0,6,5),
            	   
            (1,4,7),
            (1,7,2),
            	   
            (2,6,3),
            (2,7,6),
            	   
            (0,4,1),
            (0,5,4)]


class cell:
    def __init__(self,verts,verts_array,index):
        self.index=index
        self.verts=verts
        self.vol=0
        self.center=_vec(_avg(verts,verts_array))
        for tri in tris:
            vec = tuple(map(lambda x:_vec(verts_array[verts[x]-1]),tri))
            self.vol+=_tetraVolume(vec,self.center)
        print(self.vol)


class link:
    def __init__(self,cell1,cell2,vertex_array,face):
        ipos=cell1.center
        jpos=cell2.center
        tri1=(face[0],face[1],face[2])
        tri2=(face[0],face[2],face[3])
        tri3=(face[0],face[1],face[3])
        tri1=tuple(map(lambda x:_vec(vertex_array[x-1]),tri1))
        tri2=tuple(map(lambda x:_vec(vertex_array[x-1]),tri2))
        tri3=tuple(map(lambda x:_vec(vertex_array[x-1]),tri3))
        self.link_len=(ipos-jpos).mag
        self.area= _triArea(tri1) + _triArea(tri2)
        self.norm=_triNorm(tri3)
        print("area",self.area)
        print("norm",self.norm)
        self.first=cell1.index
        self.second=cell2.index


def _edge(v1,v2):
    return min(v1, v2), max(v1, v2)


def _avg(indices, vertArray):
    return (sum(map(lambda x: vertArray[x-1][0],indices))/len(indices),sum(map(lambda x: vertArray[x-1][1],indices))/len(indices),
    sum(map(lambda x: vertArray[x-1][2],indices))/len(indices))


def _vec(x):
    return vp.vec(x[0],x[1],x[2])


def _tetraVolume(base,vert):
    v1:vp.vec=base[1]-base[0]
    v2:vp.vec=base[2]-base[0]
    v3:vp.vec=vert-base[0]
    return 1./6*abs(v1.cross(v2).dot(v3))


def _triArea(tri):
    v1:vp.vec=tri[1]-tri[0]
    v2:vp.vec=tri[2]-tri[0]
    return 0.5*v1.cross(v2).mag


def _triNorm(tri):
    v1:vp.vec=tri[1]-tri[0]
    v2:vp.vec=tri[2]-tri[0]
    return v1.cross(v2).norm()


def readObj(lines):
    verts = list(filter(lambda x:x.split(' ')[0] in 'v', lines))
    faces = list(filter(lambda x:x.split(' ')[0] in 'f', lines))
    verts = list(map(lambda x: (float(x.split(' ')[1]), float(x.split(' ')[2]), float(x.split(' ')[3])), verts))
    faces = list(map(lambda x: (int(x.split(' ')[1].split('/')[0]),int(x.split(' ')[2].split('/')[0]),int(x.split(' ')[3].split('/')[0]),int(x.split(' ')[4].split('/')[0])), faces))
    edges = list(filter(lambda x:x.split(' ')[0] in 'l', lines))
    edges = list(map(lambda x: (int(x.split(' ')[1]), int(x.split(' ')[2])), edges))
    return verts, faces, edges


def getEdges(faces):
    edges = []
    for face in faces:
        for i in range(0,4):
            e = _edge(face[i], face[(i+1)%4])
            if e not in edges:
                edges.append(e)
    return edges

def findLoops(edges,faces):
    loops=[]
    for i in range(len(edges)):
        for j in range(i+1,len(edges)):
            skip=False
            for v in edges[i]:
                if v in edges[j]:
                    skip=True
            if skip: continue
            if _edge(edges[i][0],edges[j][0]) in edges and _edge(edges[i][1],edges[j][1]) in edges:
                loops.append((edges[i],_edge(edges[i][0],edges[j][0]),edges[j],_edge(edges[i][1],edges[j][1])))
            if _edge(edges[i][0],edges[j][1]) in edges and _edge(edges[i][1],edges[j][0]) in edges:
                loops.append((edges[i],_edge(edges[i][0],edges[j][1]),edges[j],_edge(edges[i][1],edges[j][0])))
    filtered=[]
    for loop in loops:
        passed=True
        for l in filtered:
            if set(loop) == set(l):
                passed=False
                break
        for face in faces:
            cnt=0
            for i in range(0,4):
                e = _edge(face[i], face[(i+1)%4])
                if e in loop:
                    cnt+=1
            if cnt==4: 
                passed=False
        if passed:
            filtered.append(loop)        
    return filtered

def getIncidentFaces(faces):
    edges = getEdges(faces)
    incidentFaces=[]
    for face in faces:
        incidentEdges = []
        facee=[]
        for i in range(0,4):
            e = _edge(face[i], face[(i+1)%4])
            facee.append(e)
        for vert in face:
            for edge in edges:
                if vert in edge and edge not in incidentEdges and edge not in facee:
                    incidentEdges.append(edge)
        incidentFaces.append(incidentEdges)
    return incidentFaces


def getOppositeEdges(faces):
    incidentFaces = getIncidentFaces(faces)
    oppositeFaces=[]
    for i in range(0,len(incidentFaces)):
        for j in range(i+1,len(incidentFaces)):
            e1 = incidentFaces[i]
            e2 = incidentFaces[j]
            cnt=0
            for e in e1:
                if e in e2:
                    cnt+=1
            if cnt == 4:
                oppositeFaces.append((i,j))
    return oppositeFaces

def addFaces(faces,loops):
    for loop in loops:
        v=[]
        for i in range(4):
            v.append(set(loop[i]).intersection(set(loop[(i+1)%4])))
        face=(v[0].pop(),v[1].pop(),v[2].pop(),v[3].pop())
        faces.append(face)

def getOppositeEdges2(faces,fileEdges):
    fileEdges = list(map(lambda x: _edge(x[0],x[1]),fileEdges))
    edges = getEdges(faces)
    edges = edges + fileEdges
    loops=findLoops(edges,faces)
    print("loops",loops,sep="\n")
    addFaces(faces,loops)

    oppositeFaces=[]
    for i in range(len(faces)):
        for j in range(i+1,len(faces)):
            cnt=0
            if len(set(faces[i]).intersection(set(faces[j])))!=0: continue
            for e in edges:
                if (e[0] in faces[i] and e[1] in faces[j]) or (e[1] in faces[i] and e[0] in faces[j]):
                    cnt+=1
            if cnt==4:
                oppositeFaces.append((i,j))
    return oppositeFaces

def rotate(arr):
    return arr[1:]+arr[:1]


def getCells(verts,faces,edges):
    oppositeFaces = getOppositeEdges2(faces,edges)
    cubes=[]
    edges = getEdges(faces)
    for oface in oppositeFaces:
        cubeverts=[]
        for f in list(map(lambda x: faces[x],oface)):
            for v in f:
                if v not in cubeverts:
                    cubeverts.append(v)
        while(_edge(cubeverts[0],cubeverts[5]) not in edges and _edge(cubeverts[5],cubeverts[0]) not in edges):
            cubeverts=cubeverts[:4]+rotate(cubeverts[4:])
        if (_edge(cubeverts[1],cubeverts[4]) not in edges and _edge(cubeverts[4],cubeverts[1]) not in edges):
            cubeverts=cubeverts[:4]+(rotate(list(reversed(cubeverts[4:]))))           
        if sorted(cubeverts) not in map(lambda x: sorted(x),cubes):
            cubes.append(cubeverts)
    cells = []
    index = 0
    for cube in cubes:
        cells.append(cell(cube,verts,index))
        index+=1
    return cells

def findLinks(cells,verts):
    links=[]
    for i in range(0,len(cells)):
        for j in range(i+1,len(cells)):
            commonverts=[]
            for v in cells[i].verts:
                if v in cells[j].verts:
                    commonverts.append(v)
            if len(commonverts)==4:
                links.append(link(cells[i],cells[j],verts,commonverts))
    return links

def drawCells(cells,verts):
    for cube in cells:
        tricoords=[]
        
        for tri in tris:
            vec = tuple(map(lambda x:_vec(verts[cube.verts[x]-1]),tri))
            vec1 = vec[1]-vec[0]
            vec2 = vec[2]-vec[0]
            norm = vec1.cross(vec2).norm()
            norm = vp.vec(abs(norm.x),abs(norm.y),abs(norm.z))
            tricoords.append(tuple(map(lambda x: vp.vertex(pos=x,color = norm,opacity=0.5),vec)))
        for tri in tricoords:
            vp.triangle(v0=tri[0],v1=tri[1],v2=tri[2])



vp.canvas(width=1300, height=600)


with open('t.obj') as f:
    verts, faces,edges = readObj(f.readlines())
    print(len(edges))
    cells = getCells(verts,faces,edges)
    links = findLinks(cells,verts)
    for _cell in cells:
        vp.points(pos=_cell.center)
        vp.label( pos=vp.vec(_cell.center),text="{:.2f}".format(_cell.vol))
    drawCells(cells,verts)   
    for l in links:
        vp.curve(cells[l.first].center,cells[l.second].center)
        mid=(cells[l.first].center+cells[l.second].center)/2
        vp.label(pos=mid,text="len: {:.2f}\narea: {:.2f}".format(l.link_len,l.area))